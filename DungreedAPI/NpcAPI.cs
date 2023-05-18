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
    /// <summary>
    /// Create and register NPCs.
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item><term><see cref="Add(MyNPCData)"/></term><description>Register an existing <see cref="MyNPCData"/>.</description></item>
    /// <item><term><see cref="AddNew(string, GameObjectWithComponent{NPC_Rescuable}, int, int, Sprite, GameObjectWithComponent{NPC_Basic}, BuildData, string[], string[], string, Optional{string}, Optional{string})"/></term><description>Create and register a new <see cref="MyNPCData"/>.</description></item>
    /// </list>
    /// </remarks>
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

        /// <summary>
        /// Register an existing <see cref="MyNPCData"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="npc"/> will be assigned a new valid id.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MyNPCManager"/> catalog has already loaded.</exception>
        /// <exception cref="ArgumentException"><paramref name="npc"/> is null.</exception>
        /// <param name="npc">The existing npc to add.</param>
        public static void Add(MyNPCData npc)
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

        /// <summary>
        /// Create and register a new <see cref="MyNPCData"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="MyNPCManager"/> catalog has already loaded.</exception>
        /// <param name="name">The name of this NPC.</param>
        /// <param name="prefab">A prefab for this NPC in the dungeon.</param>
        /// <param name="firstStayFloor">The first floor this NPC appears in the dungeon.</param>
        /// <param name="rescueEXPAmount">The amount of experience awarded for rescuing this NPC.</param>
        /// <param name="icon">An icon for this NPC.</param>
        /// <param name="townPrefab">A prefab for this NPC in the town.</param>
        /// <param name="connectedBuilding">A build associated with this NPC.</param>
        /// <param name="sayOnRescue">Dialogue on rescue.</param>
        /// <param name="sayThanksForBuild">Dialogue on associated building completion.</param>
        /// <param name="rescueCondition">Required flag for rescue.</param>
        /// <param name="nameKey">A localization key for the name of this NPC. Will be auto-generated if left default.</param>
        /// <param name="descriptionKey">A localization key for the description of this NPC. Will be auto-generated if left default.</param>
        /// <returns>A new <see cref="MyNPCData"/>.</returns>
        public static MyNPCData AddNew(string name, GameObjectWithComponent<NPC_Rescuable> prefab, int firstStayFloor, int rescueEXPAmount, Sprite icon,
            GameObjectWithComponent<NPC_Basic> townPrefab = null,
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
