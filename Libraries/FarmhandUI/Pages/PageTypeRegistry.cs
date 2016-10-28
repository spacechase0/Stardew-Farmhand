using System;
using System.Collections.Generic;
using System.Linq;
using Farmhand.Registries;
using Farmhand.Registries.Containers;

namespace Farmhand.UI.Pages
{
    /// <summary>
    /// Holds a reference to loaded maps.
    /// </summary>
    public static class PageTypeRegistry
    {
        private static Registry<string, Type> _pageRegistryInstance;
        private static Registry<string, Type> RegistryInstance => _pageRegistryInstance ?? (_pageRegistryInstance = new Registry<string, Type>());

        /// <summary>
        /// Returns item with matching id
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="mod">Owning mod, defaults to null</param>
        /// <returns>Matching map</returns>
        public static Type GetItem(string itemId, ModManifest mod = null)
        {
            return RegistryInstance.GetItem(mod == null ? itemId : GetModSpecificId(mod, itemId));
        }

        /// <summary>
        /// Registers item with it
        /// </summary>
        /// <param name="itemId">Id of item to register</param>
        /// <param name="item">Map to register</param>
        /// <param name="mod">Owning mod, defaults to null</param>
        public static void RegisterItem(string itemId, Type item, ModManifest mod = null)
        {
            RegistryInstance.RegisterItem(mod == null ? itemId : GetModSpecificId(mod, itemId), item);
        }

        /// <summary>
        /// Returns all registered Page types
        /// </summary>
        /// <returns>All ModXnbs</returns>
        public static IEnumerable<Type> GetRegisteredItems()
        {
            return RegistryInstance.GetRegisteredItems();
        }

        /// <summary>
        /// Removes an item with id
        /// </summary>
        /// <param name="itemId">Id to remove</param>
        /// <param name="mod">Owning mod, defaults to null</param>
        public static void UnregisterItem(string itemId, ModManifest mod = null)
        {
            RegistryInstance.UnregisterItem(mod == null ? itemId : GetModSpecificId(mod, itemId));
        }


        #region Helper Functions
        private static string GetModSpecificPrefix(ModManifest mod)
        {
            return $"\\{mod.UniqueId.ThisId}\\";
        }

        public static string GetModSpecificId(ModManifest mod, string itemId)
        {
            var modPrefix = GetModSpecificPrefix(mod);
            return $"{modPrefix}{itemId}";
        }
        #endregion
    }
}
