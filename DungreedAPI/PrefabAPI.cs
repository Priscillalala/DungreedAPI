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
    /// <remarks>
    /// <list>
    /// <item><term><see cref="CreatePrefab(string, Type[])"/></term><description>Create a new <see cref="GameObject"/> prefab.</description></item>
    /// <item><term><see cref="ClonePrefab(GameObject, string)"/></term><description>Clone an existing <see cref="GameObject"/> prefab.</description></item>
    /// </list>
    /// </remarks>
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
        /// Create a new <see cref="GameObject"/> prefab.
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

        /// <summary>
        /// Clone an existing <see cref="GameObject"/> prefab.
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
    }
}
