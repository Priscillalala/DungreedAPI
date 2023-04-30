using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;
using UnityEngine.UI;

namespace DungreedAPI
{
    public static class NpcAPI
    {
        internal static CatalogWrapper<MyNPCData> catalogWrapper;
        internal static List<Named<MyNPCData>> managedNpcs = new List<Named<MyNPCData>>();
        internal static ManagedSaveStringFormatter<Named<MyNPCData>> rescueStateFormatter = new ManagedSaveStringFormatter<Named<MyNPCData>>("NPC_Rescue_ID_{0}", x => x.Value.id, x => x.Name);
        internal static ManagedSaveStringFormatter<Named<MyNPCData>> rescueFailsFormatter = new ManagedSaveStringFormatter<Named<MyNPCData>>("NPC_Rescue_ID_{0}_FailCount", x => x.Value.id, x => x.Name);
        internal static ManagedSaveStringFormatter<Named<MyNPCData>> floorFormatter = new ManagedSaveStringFormatter<Named<MyNPCData>>("NPC_Floor_{0}", x => x.Value.id, x => x.Name);

        internal static void Init()
        {
            On.MyNPCManager.Initialize += MyNPCManager_Initialize;
            SaveInjector.beforeSave += SaveInjector_beforeSave;
        }

        private static void MyNPCManager_Initialize(On.MyNPCManager.orig_Initialize orig, MyNPCManager self)
        {
            orig(self);
            catalogWrapper = new CatalogWrapper<MyNPCData>(self.npcs, 25);
            foreach (Named<MyNPCData> npc in managedNpcs)
            {
                npc.Value.id = catalogWrapper.Add(npc.Value);
                SaveData current = SaveManager.GetCurrent();
                if (!current.HasKey(rescueStateFormatter.IdFormat(npc)) && current.TryGetValue(rescueStateFormatter.NameFormat(npc), out int state))
                {
                    current[rescueStateFormatter.IdFormat(npc)] = state;
                    current.Remove(rescueStateFormatter.NameFormat(npc));
                }
                if (!current.HasKey(rescueFailsFormatter.IdFormat(npc)) && current.TryGetValue(rescueFailsFormatter.NameFormat(npc), out int fails))
                {
                    current[rescueFailsFormatter.IdFormat(npc)] = fails;
                    current.Remove(rescueFailsFormatter.NameFormat(npc));
                }
                if (!current.HasKey(floorFormatter.IdFormat(npc)) && current.TryGetValue(floorFormatter.NameFormat(npc), out int floor))
                {
                    current[floorFormatter.IdFormat(npc)] = floor;
                    current.Remove(floorFormatter.NameFormat(npc));
                }
            }
        }

        private static void SaveInjector_beforeSave(Dictionary<string, SaveData.DataContainer> obj)
        {
            foreach (Named<MyNPCData> npc in managedNpcs)
            {
                if (obj.TryGetValue(rescueStateFormatter.IdFormat(npc), out SaveData.DataContainer rescueStateData))
                {
                    rescueStateData.key = rescueStateFormatter.NameFormat(npc);
                }
                if (obj.TryGetValue(rescueFailsFormatter.IdFormat(npc), out SaveData.DataContainer rescueFailsData))
                {
                    rescueFailsData.key = rescueFailsFormatter.NameFormat(npc);
                }
                if (obj.TryGetValue(floorFormatter.IdFormat(npc), out SaveData.DataContainer floorData))
                {
                    floorData.key = floorFormatter.NameFormat(npc);
                }
            }
        }

        public static void AddExisting(MyNPCData npc)
        {
            if (MyNPCManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            if (!npc)
            {
                throw new ArgumentException();
            }
            managedNpcs.Add(new Named<MyNPCData>(npc, npc.name));
        }

        public static MyNPCData AddNew(string name, NPC_Rescuable prefab, int firstStayFloor, int rescueEXPAmount, Sprite icon,
            GameObject townPrefab = null,
            BuildData connectedBuilding = null,
            string[] sayOnRescue = null,
            string[] sayThanksForBuild = null,
            string rescueCondition = null,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default)
        {
            if (MyNPCManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            MyNPCData npc = ScriptableObject.CreateInstance<MyNPCData>();
            npc.name = name;
            npc.npcPrefab = prefab;
            npc.npcTownPrefab = townPrefab;
            npc.firstStayFloor = firstStayFloor;
            npc.rescueEXPAmount = rescueEXPAmount;
            npc.icon = icon;
            npc.connectedBuilding = connectedBuilding;
            npc.saysOnRescue = sayOnRescue ?? Array.Empty<string>();
            npc.sayThanksForBuild = sayThanksForBuild ?? Array.Empty<string>();
            npc.condition = rescueCondition;
            npc.aName = nameKey.Exists ? nameKey.Value : $"NPC_{name}_Name";
            npc.aDescription = descriptionKey.Exists ? descriptionKey.Value : $"NPC_{name}_Description";
            npc.cantRescue = false;
            managedNpcs.Add(new Named<MyNPCData>(npc, name));
            return npc;
        }
    }
}
