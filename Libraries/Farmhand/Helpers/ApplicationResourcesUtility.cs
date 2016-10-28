using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Farmhand.Attributes;
using Farmhand.Registries.Containers;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewValley;
using xTile;
using xTile.Format;

namespace Farmhand.Helpers
{
    public class ApplicationResourcesUtility
    {
        public static IEnumerable<string> GetInternalResourceNames()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
        }

        public static Stream LoadInternalResource(string file)
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(file);
        }

        [Hook(HookType.Entry, "StardewValley.Game1", "Initialize")]
        public static void LoadInternalApiManifests()
        {
            var test = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            var manifestRead = GetInternalResourceNames().Where(n => n.EndsWith(".manifest.json"));
            foreach (var file in manifestRead)
            {
                using (var stream = LoadInternalResource(file))
                {
                    if (stream == null) continue;

                    using (var reader = new StreamReader(stream))
                    {
                        var manifestFile = reader.ReadToEnd();
                        var manifest = JsonConvert.DeserializeObject<ApplicationResourceManifest>(manifestFile);
                        manifest.LoadContent();

                        ApplicationResourceManifest.LoadedManifests.Add(manifest);
                    }
                }
            }
        }

        public static Texture2D LoadTexture(string textureFile)
        {
            using (var stream = LoadInternalResource(textureFile))
            {
                if (stream == null || Game1.graphics == null) return null;
                
                var texture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, stream);
                return texture;
            }
        }

        public static Map LoadMap(string mapFile)
        {
            throw new NotImplementedException();
        }
    }
}
