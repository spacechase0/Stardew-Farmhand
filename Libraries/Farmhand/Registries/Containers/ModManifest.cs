﻿using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Farmhand.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Farmhand.Helpers;
using xTile;
using Mono.Cecil;
using System.Security.Cryptography;
using System.IO;

namespace Farmhand.Registries.Containers
{
    public class ModManifest
    {
        public ModManifest()
        {
            ModState = ModState.Unloaded;
        }

        public static event EventHandler BeforeLoaded;
        public static event EventHandler AfterLoaded;

        private static readonly SHA1CryptoServiceProvider Sha1 = new SHA1CryptoServiceProvider();

        [JsonConverter(typeof(UniqueIdConverter))]
        public UniqueId<string> UniqueId { get; set; }

        public string ModDll { get; set; }

        public string Name { get; set; }    
            
        public string Author { get; set; }

        public Version Version { get; set; }

        public string Description { get; set; }
        public List<ModDependency> Dependencies { get; set; }

        public ManifestContent Content { get; set; }

        private string _configurationFile;
        public string ConfigurationFile
        {
            get { return $"{ModDirectory}\\{_configurationFile}"; }
            set { _configurationFile = value; }
        }

        #region SMAPI Compatibility

        public bool IsSmapiMod { get; set; }

        public string Authour
        {
            set { Author = value; }
        }

        public string EntryDll
        {
            set
            {
                var index = value.LastIndexOf(".dll", StringComparison.Ordinal);
                ModDll = index != -1 ? value.Remove(index) : value;
            }
        }
        public bool PerSaveConfigs { get; set; }

        #endregion

        #region Manifest Instance Data

        [JsonIgnore]
        public ModState ModState { get; set; }
        [JsonIgnore]
        public Exception LastException { get; set; }
        [JsonIgnore]
        public bool HasDll => !string.IsNullOrWhiteSpace(ModDll);
        [JsonIgnore]
        public bool HasConfig => HasDll && !string.IsNullOrWhiteSpace(ConfigurationFile);
        [JsonIgnore]
        public bool HasContent => Content != null && Content.HasContent;
        [JsonIgnore]
        public Assembly ModAssembly { get; set; }
        [JsonIgnore]
        public object Instance { get; set; }
        //[JsonIgnore]
        //public StardewModdingAPI.Mod SmapiInstance { get; set; }
        [JsonIgnore]
        public string ModDirectory { get; set; }

        internal bool LoadModDll()
        {
            if (Instance != null)
            {
                throw new Exception("Error! Mod has already been loaded!");
            }

            try
            {
                ModAssembly = Assembly.Load(GetDllBytes());
                if (ModAssembly.GetTypes().Count(x => x.BaseType == typeof(Mod)) > 0)
                {
                    var type = ModAssembly.GetTypes().First(x => x.BaseType == typeof(Mod));
                    Mod instance = (Mod)ModAssembly.CreateInstance(type.ToString());
                    if (instance != null)
                    {
                        instance.ModSettings = this;
                        instance.Entry();
                    }
                    Instance = instance;
                    Log.Verbose ($"Loaded mod dll: {Name}");
                }
                else
                {
                    var types = ModAssembly.GetTypes();
                    foreach (var layer in ModLoader.CompatibilityLayers)
                    {
                        if (!layer.ContainsOurModType(types)) continue;

                        Instance = layer.LoadMod(ModAssembly, types, this);
                        Log.Verbose($"Loaded mod dll: {Name}");
                        break;
                    }
                }
               
                if(Instance == null)
                {
                    throw new Exception("Invalid Mod DLL");
                }
            }
            catch (Exception ex)
            {
                var exception = ex as ReflectionTypeLoadException;
                if (exception != null)
                {
                    foreach (var e in exception.LoaderExceptions)
                    {
                        Log.Exception("loaderexceptions entry: " +
                            $"{e.Message} ${e.Source} ${e.TargetSite} ${e.StackTrace} ${e.Data}", e);
                    }
                }
                Log.Exception("Error loading mod DLL", ex);
                //throw new Exception(string.Format($"Failed to load mod '{modDllPath}'\n\t-{ex.Message}\n\t\t-{ex.StackTrace}"), ex);
            }

            return Instance != null;
        }

        internal byte[] GetDllBytes()
        {
            var modDllPath = $"{ModDirectory}\\{ModDll}";

            if (!modDllPath.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
            {
                modDllPath += ".dll";
            }

            // When a SMAPI mod is still referencing the vanilla assembly,
            // it doesn't see the Farmhand game. Everything will still be 
            // in the state for when the assembly would first be loaded.
            // This will fix the references so the mods will actually work.
            //
            // For loading mods from other platforms:
            // There are also problems with XNA <=> Mono. Simply replacing
            // the assembly name isn't really possible in this case, since
            // all three XNA assemblies map to one Mono framework.
            // (Trust me, I tried. Suddenly it looks for the definition of
            // Vector2 inside the Farmhand assembly, or somewhere else
            // bizarre.)
            var bytes = File.ReadAllBytes(modDllPath);

            var asm = AssemblyDefinition.ReadAssembly(new MemoryStream(bytes, false));

            var shouldFix = false;
            var toRemove = new List<AssemblyNameReference>();
            foreach (var asmRef in asm.MainModule.AssemblyReferences)
            {
                // We only want to go to the effort of fixing everything if 
                // there is a reference that even needs fixing.
                // Also, the bad references will need to be removed.
                if (ReferenceHelper.MatchesPlatform(asmRef)) continue;

                shouldFix = true;
                toRemove.Add(asmRef);
            }
            foreach (var @ref in toRemove)
            {
                // However, we can't just simply remove the old references.
                // The indices mess up and really weird stuff happens (see
                // two comment blocks ago). Placing a dummy re-reference of
                // ourself SEEMS to fix that.
                var index = asm.MainModule.AssemblyReferences.IndexOf(@ref);
                asm.MainModule.AssemblyReferences.Remove(@ref);
                asm.MainModule.AssemblyReferences.Insert(index, ReferenceHelper.ThisRef);
            }

            if (shouldFix)
            {
                if (Program.Config.CachePorts)
                {
                    // We want to cache any 'fixed' assemblies so that we don't have to
                    // go through and fix everything again. However if the mod is updated
                    // or something, it will need to be fixed again.
                    string check = Convert.ToBase64String(Sha1.ComputeHash(bytes));
                    check = check.Replace("=", "").Replace('+', '-').Replace('/', '_'); // Fix for valid file name. = is just padding
                    string checkPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                    "StardewValley", "cache", ModDll + "-" + check + ".dll");
                    if (File.Exists(checkPath))
                    {
                        try
                        {
                            bytes = File.ReadAllBytes(checkPath);
                        }
                        catch (Exception ex)
                        {
                            Log.Exception($"Exception reading cached DLL {checkPath}", ex);
                            bytes = FixDll(asm, checkPath);
                        }
                    }
                    else bytes = FixDll(asm, checkPath);
                }
                else bytes = FixDll(asm, null);
            }

            return bytes;
        }

        private static byte[] FixDll( AssemblyDefinition asm, string cachePath )
        {
            DefinitionResolver.Fix(asm);

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                asm.Write(stream);
                bytes = stream.GetBuffer();
            }

            if (cachePath != null)
            {
                try
                {
                    var dir = Path.GetDirectoryName(cachePath);
                    if (dir != null)
                    {
                        if (!Directory.Exists(dir))
                            Directory.CreateDirectory(dir);
                        File.WriteAllBytes(cachePath, bytes);
                    }
                    else
                    {
                        throw new Exception("Path.GetDirectoryName(cachePath) returned null");
                    }
                }
                catch (Exception ex)
                {
                    Log.Exception($"Exception caching fixed DLL {cachePath}", ex);
                }
            }

            return bytes;
        }

        internal void LoadContent()
        {
            if (Content == null)
                return;

            Log.Verbose("Loading Content");
            if (Content.Textures != null)
            {
                foreach (var texture in Content.Textures)
                {
                    texture.AbsoluteFilePath = $"{ModDirectory}\\{Constants.ModContentDirectory}\\{texture.File}";

                    if (!texture.Exists())
                    {
                        throw new Exception($"Missing Texture: {texture.AbsoluteFilePath}");
                    }

                    Log.Verbose($"Registering new texture: {texture.Id}");
                    TextureRegistry.RegisterItem(texture.Id, texture, this);
                }
            }

            if (Content.Maps != null)
            {
                foreach (var map in Content.Maps)
                {
                    map.AbsoluteFilePath = $"{ModDirectory}\\{Constants.ModContentDirectory}\\{map.File}";

                    if (!map.Exists())
                    {
                        throw new Exception($"Missing map: {map.AbsoluteFilePath}");
                    }

                    Log.Verbose($"Registering new map: {map.Id}");
                    MapRegistry.RegisterItem(map.Id, map, this);
                }
            }

            if (Content.Xnb == null) return;

            foreach (var file in Content.Xnb)
            {
                if (file.IsXnb)
                {
                    file.AbsoluteFilePath = $"{ModDirectory}\\{Constants.ModContentDirectory}\\{file.File}";
                }
                file.OwningMod = this;
                if (!file.Exists(this))
                {
                    if (file.IsXnb)
                        throw new Exception($"Replacement File: {file.AbsoluteFilePath}");
                    if (file.IsTexture)
                        throw new Exception($"Replacement Texture: {file.Texture}");
                }
                Log.Verbose("Registering new texture XNB override");
                XnbRegistry.RegisterItem(file.Original, file, this);
            }
        }
        
        public Texture2D GetTexture(string id)
        {
            return TextureRegistry.GetItem(id, this).Texture;
        }

        public Map GetMap(string id)
        {
            return MapRegistry.GetItem(id, this).Map;
        }

        #endregion

        public void OnBeforeLoaded()
        {
            BeforeLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void OnAfterLoaded()
        {
            AfterLoaded?.Invoke(this, EventArgs.Empty);
        }
    }
}
