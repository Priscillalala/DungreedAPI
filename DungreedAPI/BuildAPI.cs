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
    public static class BuildAPI
    {
        internal static CatalogWrapper<BuildData> catalogWrapper;
        internal static List<Named<BuildData>> managedBuilds = new List<Named<BuildData>>();
        internal static ManagedSaveStringFormatter<Named<BuildData>> stateFormatter = new ManagedSaveStringFormatter<Named<BuildData>>("Builder_{0}_state", x => x.Value.id, x => x.Name);
        internal static bool hasLoadedBuilds = false;

        internal static void Init()
        {
            On.BuildManager.Initialize += BuildManager_Initialize;
            SaveInjector.beforeSave += SaveInjector_beforeSave;
        }

        private static void BuildManager_Initialize(On.BuildManager.orig_Initialize orig, BuildManager self)
        {
            orig(self);
            catalogWrapper = new CatalogWrapper<BuildData>(self.data, 16);
            foreach (Named<BuildData> build in managedBuilds)
            {
                if (build.Value.noUse)
                {
                    continue;
                }
                build.Value.id = catalogWrapper.Add(build.Value);
                SaveData current = SaveManager.GetCurrent();
                if (!current.HasKey(stateFormatter.IdFormat(build)) && current.TryGetValue(stateFormatter.NameFormat(build), out int state))
                {
                    current[stateFormatter.IdFormat(build)] = state;
                    current.Remove(stateFormatter.NameFormat(build));
                }
            }
            hasLoadedBuilds = true;
        }

        private static void SaveInjector_beforeSave(Dictionary<string, SaveData.DataContainer> obj)
        {
            foreach (Named<BuildData> build in managedBuilds)
            {
                if (obj.TryGetValue(stateFormatter.IdFormat(build), out SaveData.DataContainer stateData))
                {
                    stateData.key = stateFormatter.NameFormat(build);
                }
            }
        }

        public static void AddExisting(BuildData build)
        {
            if (hasLoadedBuilds)
            {
                throw new InvalidOperationException();
            }
            if (!build)
            {
                throw new ArgumentException();
            }
            managedBuilds.Add(new Named<BuildData>(build, build.name));
        }

        public static BuildData AddNew(string name, BuildObject prefab, int buildCost, Sprite icon, Sprite npcIcon,
            MyItemData[] functionalUnlockItems = null,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default,
            Optional<string> subDescriptionKey = default)
        {
            if (hasLoadedBuilds)
            {
                throw new InvalidOperationException();
            }
            BuildData build = ScriptableObject.CreateInstance<BuildData>();
            build.name = name;
            build.prefab = prefab;
            build.buildCost = buildCost;
            build.icon = icon;
            build.npcIcon = npcIcon;
            build.functionalUnlockItem = functionalUnlockItems ?? Array.Empty<MyItemData>();
            build.aName = nameKey.Exists ? nameKey.Value : $"Build_{name}_Name";
            build.description = descriptionKey.Exists ? descriptionKey.Value : $"Build_{name}_Description";
            build.subDescription = subDescriptionKey.Exists ? subDescriptionKey.Value : $"Build_{name}_SubDescription";
            build.noUse = false;
            managedBuilds.Add(new Named<BuildData>(build, name));
            return build;
        }
    }
}
