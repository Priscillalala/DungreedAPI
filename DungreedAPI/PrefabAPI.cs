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
        
        public static GameObject InstantiateClone(GameObject source, string name)
        {
            GameObject clone = UnityEngine.Object.Instantiate(source, prefabsHolder.transform);
            clone.name = name;
            return clone;
        }
    }
}
