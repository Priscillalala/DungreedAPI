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
    /// Create and register training center ability paths.
    /// </summary>
    public static class AbilityPathAPI
    {
        internal static CatalogWrapper<MyAbilityData> catalogWrapper;
        internal static List<Named<MyAbilityPathData>> managedAbilityPaths = new List<Named<MyAbilityPathData>>();
        internal static ManagedSaveStringFormatter<Named<MyAbilityPathData>> levelFormatter = new ManagedSaveStringFormatter<Named<MyAbilityPathData>>("Player_Ability_{0}_LV", x => x.Value.id, x => x.Name);

        internal static void Init()
        {
            On.MyAbilityManager.Initialize += MyAbilityManager_Initialize;
            On.UI_AbilityPanel.Awake += UI_AbilityPanel_Awake;
            SaveInjector.beforeSave += SaveInjector_beforeSave;
        }

        private static void MyAbilityManager_Initialize(On.MyAbilityManager.orig_Initialize orig, MyAbilityManager self)
        {
            orig(self);
            catalogWrapper = new CatalogWrapper<MyAbilityData>(self.abilities, 7);
            SaveData current = SaveManager.GetCurrent();
            foreach (Named<MyAbilityPathData> abilityPath in managedAbilityPaths)
            {
                abilityPath.Value.id = catalogWrapper.Add(abilityPath.Value);
                if (!current.HasKey(levelFormatter.IdFormat(abilityPath)) && current.TryGetValue(levelFormatter.NameFormat(abilityPath), out int level))
                {
                    current[levelFormatter.IdFormat(abilityPath)] = level;
                    current.Remove(levelFormatter.NameFormat(abilityPath));
                }
            }
        }

        private static void UI_AbilityPanel_Awake(On.UI_AbilityPanel.orig_Awake orig, UI_AbilityPanel self)
        {
            orig(self);
            if (self.icons.Length <= 0 || managedAbilityPaths.Count <= 0)
            {
                return;
            }
            GameObject icon = self.icons[0].gameObject;
            int length = self.icons.Length;
            Array.Resize(ref self.icons, length + managedAbilityPaths.Count);
            for (int i = 0; i < managedAbilityPaths.Count; i++)
            {
                UI_AbilityIcon_New newIcon = UnityEngine.Object.Instantiate(icon, icon.transform.parent).GetComponent<UI_AbilityIcon_New>();
                newIcon.abilityID = managedAbilityPaths[i].Value.id;
                newIcon.GetComponent<Image>().sprite = managedAbilityPaths[i].Value.background;
                self.icons[length + i] = newIcon;
            }
        }

        private static void SaveInjector_beforeSave(Dictionary<string, SaveData.DataContainer> obj)
        {
            foreach (Named<MyAbilityPathData> abilityPath in managedAbilityPaths)
            {
                if (obj.TryGetValue(levelFormatter.IdFormat(abilityPath), out SaveData.DataContainer levelData))
                {
                    levelData.key = levelFormatter.NameFormat(abilityPath);
                }
            }
        }

        /// <summary>
        /// Registers an existing <see cref="MyAbilityPathData"/> to the <see cref="MyAbilityManager"/> catalog.
        /// </summary>
        /// <remarks>
        /// <paramref name="abilityPath"/> will be assigned a new valid id.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MyAbilityManager"/> catalog has already loaded.</exception>
        /// <exception cref="ArgumentException"><paramref name="abilityPath"/> is null.</exception>
        /// <param name="abilityPath">The existing ability path to add.</param>
        public static void AddExisting(MyAbilityPathData abilityPath)
        {
            if (MyAbilityManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            if (!abilityPath)
            {
                throw new ArgumentException();
            }
            managedAbilityPaths.Add(new Named<MyAbilityPathData>(abilityPath, abilityPath.name));
        }

        /// <summary>
        /// Creates a new <see cref="MyAbilityPathData"/> that is registered to the <see cref="MyAbilityManager"/> catalog.
        /// </summary>
        /// <remarks>
        /// <list>
        /// <item><term><paramref name="name"/></term><description>The name of this ability path.</description></item>
        /// <item><term><paramref name="level5Perk"/></term><description>A perk unlocked at ability path level 5.</description></item>
        /// <item><term><paramref name="level10Perk"/></term><description>A perk unlocked at ability path level 10.</description></item>
        /// <item><term><paramref name="level20Perk"/></term><description>A perk unlocked at ability path level 20.</description></item>
        /// <item><term><paramref name="effects"/></term><description>Status effects applied for each level of this ability path.</description></item>
        /// <item><term><paramref name="background"/></term><description>A background image for this ability path.</description></item>
        /// <item><term><paramref name="nameKey"/></term><description>A localization key for the name of this ability path. Will be auto-generated if left default.</description></item>
        /// <item><term><paramref name="descriptionKey"/></term><description>A localization key for the description of this ability path. Will be auto-generated if left default.</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MyAbilityManager"/> catalog has already loaded.</exception>
        /// <param name="name">The name of this ability path.</param>
        /// <param name="level5Perk">A perk unlocked at ability path level 5.</param>
        /// <param name="level10Perk">A perk unlocked at ability path level 10.</param>
        /// <param name="level20Perk">A perk unlocked at ability path level 20.</param>
        /// <param name="effects">Status effects applied for each level of this ability path.</param>
        /// <param name="background">A background image for this ability path.</param>
        /// <param name="nameKey">A localization key for the name of this ability path. Will be auto-generated if left default.</param>
        /// <param name="descriptionKey">A localization key for the description of this ability path. Will be auto-generated if left default.</param>
        /// <returns>A new <see cref="MyAbilityPathData"/>.</returns>
        public static MyAbilityPathData AddNew(string name, AbilityPerk level5Perk, AbilityPerk level10Perk, AbilityPerk level20Perk,
            string[] effects = null,
            Sprite background = null,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default)
        {
            if (MyAbilityManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            MyAbilityPathData ability = ScriptableObject.CreateInstance<MyAbilityPathData>();
            ability.name = name;
            ability.effects = effects ?? Array.Empty<string>();
            ability.background = background;
            ability.lv6Perk = level5Perk.prefab;
            ability.lv6PerkInctive = level5Perk.inactive;
            ability.lv6PerkActive = level5Perk.active;
            ability.lv6PerkSelected = level5Perk.active;
            ability.lv13Perk = level10Perk.prefab;
            ability.lv13PerkInctive = level10Perk.inactive;
            ability.lv13PerkActive = level10Perk.active;
            ability.lv13PerkSelected = level10Perk.active;
            ability.lv20Perk = level20Perk.prefab;
            ability.lv20PerkInctive = level20Perk.inactive;
            ability.lv20PerkActive = level20Perk.active;
            ability.lv20PerkSelected = level20Perk.active;
            ability.aName = nameKey.Exists ? nameKey.Value : $"Abilities_{name}_Name";
            ability.aDescription = descriptionKey.Exists ? descriptionKey.Value : $"Abilities_{name}_Description";
            managedAbilityPaths.Add(new Named<MyAbilityPathData>(ability, name));
            return ability;
        }
    }
}
