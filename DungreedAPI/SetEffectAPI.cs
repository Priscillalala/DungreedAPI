using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;

namespace DungreedAPI
{
    public static class SetEffectAPI
    {
        internal static CatalogWrapper<MySetEffectData> catalogWrapper;
        internal static List<Named<MySetEffectData>> managedSetEffects = new List<Named<MySetEffectData>>();
        internal static ManagedSaveStringFormatter<Named<MySetEffectData>> fullyUnlockedFormatter = new ManagedSaveStringFormatter<Named<MySetEffectData>>("Unlocked_Set_{0}", x => x.Value.id, x => x.Name);
        internal static bool hasLoadedSetEffects = false;
        internal static AudioClip defaultSetEffectClip;

        internal static void Init()
        {
            defaultSetEffectClip = Resources.Load<MySetEffectData>("seteffects/V01_Combat police").setEffectClip;
            On.MySetEffectManager.Initialize += MySetEffectManager_Initialize;
            SaveInjector.beforeSave += SaveInjector_beforeSave;
        }

        private static void MySetEffectManager_Initialize(On.MySetEffectManager.orig_Initialize orig, MySetEffectManager self)
        {
            orig(self);
            catalogWrapper = new CatalogWrapper<MySetEffectData>(self.setEffects, 49);
            foreach (Named<MySetEffectData> setEffect in managedSetEffects)
            {
                if (setEffect.Value.inactivated)
                {
                    continue;
                }
                setEffect.Value.id = catalogWrapper.Add(setEffect.Value);
                if (TryFullyUnlockManagedSetEffect(setEffect))
                {
                    self.fullUnlockedSetIDs.Add(setEffect.Value.id);
                }
            }
            hasLoadedSetEffects = true;
        }

        internal static bool TryFullyUnlockManagedSetEffect(Named<MySetEffectData> setEffect)
        {
            SaveData current = SaveManager.GetCurrent();
            if (!current.TryGetValue(fullyUnlockedFormatter.IdFormat(setEffect), out bool unlocked) && current.TryGetValue(fullyUnlockedFormatter.NameFormat(setEffect), out unlocked))
            {
                current[fullyUnlockedFormatter.IdFormat(setEffect)] = unlocked;
                current.Remove(fullyUnlockedFormatter.NameFormat(setEffect));
            }
            return unlocked;
        }

        private static void SaveInjector_beforeSave(Dictionary<string, SaveData.DataContainer> obj)
        {
            foreach (Named<MySetEffectData> setEffect in managedSetEffects)
            {
                if (obj.TryGetValue(fullyUnlockedFormatter.IdFormat(setEffect), out SaveData.DataContainer unlockedData))
                {
                    unlockedData.key = fullyUnlockedFormatter.NameFormat(setEffect);
                }
            }
        }

        public static void Add(MySetEffectData setEffect)
        {
            if (hasLoadedSetEffects)
            {
                throw new InvalidOperationException();
            }
            if (!setEffect)
            {
                throw new ArgumentException();
            }
            managedSetEffects.Add(new Named<MySetEffectData>(setEffect, setEffect.name));
        }

        public static MySetEffectData AddNew(string name, Sprite icon,
            MyWeaponData mainWeapon = null,
            MyWeaponData offWeapon = null,
            MyAccessoryData[] accessories = null,
            string[] effects = null,
            GameObjectWithComponent<Player_SetEffect> prefab = null,
            SkillData primarySkill = null,
            SkillData secondarySkill = null,
            Optional<AudioClip> setEffectClip = default,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default)
        {
            if (hasLoadedSetEffects)
            {
                throw new InvalidOperationException();
            }
            MySetEffectData setEffect = ScriptableObject.CreateInstance<MySetEffectData>();
            setEffect.name = name;
            setEffect.icon = icon;
            setEffect.mainWeapon = mainWeapon;
            setEffect.offWeapon = offWeapon;
            setEffect.accessories = accessories ?? Array.Empty<MyAccessoryData>();
            setEffect.effects = effects ?? Array.Empty<string>();
            setEffect.setEffectResource = prefab;
            setEffect.skillQ = primarySkill;
            setEffect.skillE = secondarySkill;
            setEffect.setEffectClip = setEffectClip.Exists ? setEffectClip.Value : defaultSetEffectClip;
            setEffect.aName = nameKey.Exists ? nameKey.Value : $"SetEffect_{name}_Name";
            setEffect.aDescription = descriptionKey.Exists ? descriptionKey.Value : $"SetEffect_{name}_Description";
            setEffect.inactivated = false;
            managedSetEffects.Add(new Named<MySetEffectData>(setEffect, name));
            return setEffect;
        }
    }
}
