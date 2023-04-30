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
    public static class AbilityAPI
    {
        internal static CatalogWrapper<MyAbilityData> catalogWrapper;
        internal static List<Named<MyFullAbilityData>> managedAbilities = new List<Named<MyFullAbilityData>>();
        internal static ManagedSaveStringFormatter<Named<MyFullAbilityData>> levelFormatter = new ManagedSaveStringFormatter<Named<MyFullAbilityData>>("Player_Ability_{0}_LV", x => x.Value.id, x => x.Name);

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
            foreach (Named<MyFullAbilityData> ability in managedAbilities)
            {
                ability.Value.id = catalogWrapper.Add(ability.Value);
                if (!current.HasKey(levelFormatter.IdFormat(ability)) && current.TryGetValue(levelFormatter.NameFormat(ability), out int level))
                {
                    current[levelFormatter.IdFormat(ability)] = level;
                    current.Remove(levelFormatter.NameFormat(ability));
                }
            }
        }

        private static void UI_AbilityPanel_Awake(On.UI_AbilityPanel.orig_Awake orig, UI_AbilityPanel self)
        {
            orig(self);
            if (self.icons.Length <= 0 || managedAbilities.Count <= 0)
            {
                return;
            }
            GameObject icon = self.icons[0].gameObject;
            int length = self.icons.Length;
            Array.Resize(ref self.icons, length + managedAbilities.Count);
            for (int i = 0; i < managedAbilities.Count; i++)
            {
                UI_AbilityIcon_New newIcon = UnityEngine.Object.Instantiate(icon, icon.transform.parent).GetComponent<UI_AbilityIcon_New>();
                newIcon.abilityID = managedAbilities[i].Value.id;
                newIcon.GetComponent<Image>().sprite = managedAbilities[i].Value.background;
                self.icons[length + i] = newIcon;
            }
        }

        private static void SaveInjector_beforeSave(Dictionary<string, SaveData.DataContainer> obj)
        {
            foreach (Named<MyFullAbilityData> ability in managedAbilities)
            {
                if (obj.TryGetValue(levelFormatter.IdFormat(ability), out SaveData.DataContainer levelData))
                {
                    levelData.key = levelFormatter.NameFormat(ability);
                }
            }
        }

        public static void AddExisting(MyFullAbilityData ability)
        {
            if (MyAbilityManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            if (!ability)
            {
                throw new ArgumentException();
            }
            managedAbilities.Add(new Named<MyFullAbilityData>(ability, ability.name));
        }

        public static MyFullAbilityData AddNew(string name, AbilityPerk level5, AbilityPerk level10, AbilityPerk level20,
            string[] effects = null,
            Sprite background = null,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default)
        {
            if (MyAbilityManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            MyFullAbilityData ability = ScriptableObject.CreateInstance<MyFullAbilityData>();
            ability.name = name;
            ability.effects = effects ?? Array.Empty<string>();
            ability.background = background;
            ability.lv6Perk = level5.perk;
            ability.lv6PerkInctive = level5.inactive;
            ability.lv6PerkActive = level5.active;
            ability.lv6PerkSelected = level5.active;
            ability.lv13Perk = level10.perk;
            ability.lv13PerkInctive = level10.inactive;
            ability.lv13PerkActive = level10.active;
            ability.lv13PerkSelected = level10.active;
            ability.lv20Perk = level20.perk;
            ability.lv20PerkInctive = level20.inactive;
            ability.lv20PerkActive = level20.active;
            ability.lv20PerkSelected = level20.active;
            ability.aName = nameKey.Exists ? nameKey.Value : $"Abilities_{name}_Name";
            ability.aDescription = descriptionKey.Exists ? descriptionKey.Value : $"Abilities_{name}_Description";
            managedAbilities.Add(new Named<MyFullAbilityData>(ability, name));
            return ability;
        }
    }
}
