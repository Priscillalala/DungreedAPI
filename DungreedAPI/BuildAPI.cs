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
    /// Create and register town builds.
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item><term><see cref="Add(BuildData)"/></term><description>Register an existing <see cref="BuildData"/>.</description></item>
    /// <item><term><see cref="AddNew(string, GameObjectWithComponent{BuildObject}, int, Sprite, Sprite, MyItemData[], Optional{string}, Optional{string}, Optional{string})"/></term><description>Create and register a new <see cref="BuildData"/>.</description></item>
    /// </list>
    /// </remarks>
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

        /// <summary>
        /// Register an existing <see cref="BuildData"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="build"/> will be assigned a new valid id.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The <see cref="BuildManager"/> catalog has already loaded.</exception>
        /// <exception cref="ArgumentException"><paramref name="build"/> is null.</exception>
        /// <param name="build">The existing build to add.</param>
        public static void Add(BuildData build)
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

        /// <summary>
        /// Create and register a new <see cref="BuildData"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="BuildManager"/> catalog has already loaded.</exception>
        /// <param name="name">The name of this build.</param>
        /// <param name="prefab">A prefab for this build.</param>
        /// <param name="buildCost">The cost to complete this build.</param>
        /// <param name="icon">An icon for this build.</param>
        /// <param name="npcIcon">An icon for this build's NPC.</param>
        /// <param name="functionalUnlockItems">Items unlocked by the completion of this build.</param>
        /// <param name="nameKey">A localization key for the name of this build. Will be auto-generated if left default.</param>
        /// <param name="descriptionKey">A localization key for the description of this build. Will be auto-generated if left default.</param>
        /// <param name="subDescriptionKey">A localization key for the sub-description of this build. Will be auto-generated if left default.</param>
        /// <returns>A new <see cref="BuildData"/>.</returns>
        public static BuildData AddNew(string name, GameObjectWithComponent<BuildObject> prefab, int buildCost, Sprite icon, Sprite npcIcon,
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
