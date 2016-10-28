using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Farmhand.Attributes;
using Farmhand.Registries.Containers;
using Farmhand.UI.Pages.Components;
using Newtonsoft.Json.Linq;
using Farmhand.Helpers;
using Farmhand.Registries;
using Farmhand.UI.PageDefinitions.Controls;
using Farmhand.UI.Pages.Controls;
using Farmhand.UI.Pages.Parameters;
using Farmhand.UI.Pages.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewValley;

namespace Farmhand.UI.Pages
{
    class Page
    {
        public static List<IPageComponent> Pages = new List<IPageComponent>();

        [Hook(HookType.Exit, "StardewValley.Game1", "LoadContent")]
        public static void LoadInternalPages()
        {
            foreach (var form in ApplicationResourcesUtility.LoadInternalResources("^\\s*.*\\.PageForm\\.json*\\s*$"))
            {
                using (var reader = new StreamReader(form))
                {
                    var manifestFile = reader.ReadToEnd();
                    var component = JsonConvert.DeserializeObject<ComponentDefinition>(manifestFile);
                }
                form.Dispose();
            }
                
            using (var modMenu = ApplicationResourcesUtility.LoadInternalResource("Farmhand.UI.ApiPages.ModMenu.json"))
            {
                Pages.Add(LoadComponentFromManifest(modMenu));
            }

            OnAfterGameInitialise();
        }

        public static void OnAfterGameInitialise()
        {
            var texture = TextureRegistry.GetItem("FarmhandUI.modTitleMenu", null);
            Farmhand.UI.TitleMenu.RegisterNewTitleButton(new Farmhand.UI.TitleMenu.CustomTitleOption
            {
                Key = "Mods",
                Texture = texture.Texture,
                TextureSourceRect = new Rectangle(222, 187, 74, 58),
                OnClick = OnModMenuItemClicked
            });
        }

        public static void OnModMenuItemClicked(Farmhand.UI.TitleMenu menu, string choice)
        {
            if (choice != "Mods") return;

            menu.StartMenuTransitioning();
            Game1.playSound("select");
            menu.SetSubmenu((ClickableMenu)Pages[0]);
        }

        public static IPageComponent LoadComponentFromManifest(string manifest)
        {
            using (var fileStream = File.OpenRead(manifest))
            {
                return LoadComponentFromManifest(fileStream);
            }
        }

        public static IPageComponent LoadComponentFromManifest(Stream manifest)
        {
            string manifestContents;
            using (var sr = new StreamReader(manifest))
            {
                manifestContents = sr.ReadToEnd();
            }

            var mainObj = JObject.Parse(manifestContents);
            var component = GatherComponentHierarchy(mainObj);
            if(component == null)
                throw new Exception("Error loading component hierarchy. Return value is null");

            var pageComponent = component as IPageComponent;
            if (pageComponent == null)
                throw new Exception("Error. Base component in a page hierarchy must be derive type IPageComponent");
            
            return pageComponent;
        }

        private static IComponent InstanciateComponentFromJObject(JObject jsonObject)
        {
            var type = (string)((JValue)jsonObject.GetValue("type")).Value;
            var pageParams = jsonObject.GetValue("params");

            var paramType = PageParameterTypeRegistry.GetItem(type);
            var pageType = PageTypeRegistry.GetItem(type);
            var paramObject = pageParams.ToObject(paramType);

            return Activator.CreateInstance(pageType, paramObject) as IComponent;
        }

        private static IComponent GatherComponentHierarchy(JObject jsonObject)
        {
            var nodesToEvaluate = new Stack<KeyValuePair<IComponent, JObject>>();
            var baseObject = InstanciateComponentFromJObject(jsonObject);
            nodesToEvaluate.Push(new KeyValuePair<IComponent, JObject>(baseObject, jsonObject));
            
            do
            {
                var node = nodesToEvaluate.Pop();

                var component = node.Key;
                var childJsonNode = node.Value.GetValue("children");

                if (childJsonNode == null || !childJsonNode.Children().Any())
                    continue;

                // if(childJsonNode.GetType() != typeof(JArray))
                //    throw new Exception($"Property 'children' in object {component.Type} must be an array");

                var childArray = (JArray)childJsonNode;

                foreach (var child in childArray)
                {
                    var childObj = child as JObject;
                    if (childObj == null)
                        continue;

                    var childComponent = InstanciateComponentFromJObject(childObj);
                    nodesToEvaluate.Push(new KeyValuePair<IComponent, JObject>(childComponent, childObj));
                }

            } while (nodesToEvaluate.Count > 0);

            return baseObject;
        }

        internal static void DrawHierarchy(IComponent baseComponent, SpriteBatch sb)
        {
            foreach (var child in baseComponent.Children)
            {
                child.Draw(sb);
            }
        }
    }
}
