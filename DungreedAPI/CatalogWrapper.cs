using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;

namespace DungreedAPI
{
    public class CatalogWrapper<TValue>
    {
        private readonly Dictionary<int, TValue> catalog;
        private int currentId;
        public CatalogWrapper(Dictionary<int, TValue> catalog, int idFloor)
        {
            this.catalog = catalog;
            currentId = idFloor;
        }
        public int Add(TValue value)
        {
            do
            {
                currentId++;
            } while (catalog.ContainsKey(currentId));
            catalog.Add(currentId, value);
            return currentId;
        }
    }
}
