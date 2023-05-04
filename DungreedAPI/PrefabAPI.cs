using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using System.Xml;
using System.IO;

namespace DungreedAPI
{
    /// <summary>
    /// Create and clone prefabs.
    /// </summary>
    public static class PrefabAPI
    {
        internal static Transform prefabsHolder
        {
            get
            {
                if (!_prefabsHolder)
                {
                    _prefabsHolder = new GameObject("PrefabAPI_PrefabsHolder");
                    _prefabsHolder.SetActive(false);
                    UnityEngine.Object.DontDestroyOnLoad(_prefabsHolder);
                }
                return _prefabsHolder.transform;
            }
        }
        private static GameObject _prefabsHolder;

        internal static void Init() { }
        
        /// <summary>
        /// Clone an existing prefab <see cref="GameObject"/>.
        /// </summary>
        /// <param name="source">A prefab to clone.</param>
        /// <param name="name">The name of the new prefab.</param>
        /// <returns>The newly created prefab.</returns>
        public static GameObject ClonePrefab(GameObject source, string name)
        {
            GameObject clone = UnityEngine.Object.Instantiate(source, prefabsHolder.transform);
            clone.name = name;
            return clone;
        }

        /// <summary>
        /// Create a new prefab <see cref="GameObject"/>.
        /// </summary>
        /// <param name="name">The name of this prefab.</param>
        /// <param name="components">Component types attached to this prefab.</param>
        /// <returns>The newly created prefab.</returns>
        public static GameObject CreatePrefab(string name, params Type[] components)
        {
            GameObject prefab = new GameObject(name, components);
            prefab.transform.SetParent(prefabsHolder.transform);
            return prefab;
        }
    }
}
