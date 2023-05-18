using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;

namespace DungreedAPI
{
    /// <summary>
    /// Create and register item set effects.
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item><term><see cref="Add(MySetEffectData)"/></term><description>Register an existing <see cref="MySetEffectData"/>.</description></item>
    /// <item><term><see cref="AddNew(string, Sprite, MyWeaponData, MyWeaponData, MyAccessoryData[], string[], GameObjectWithComponent{Player_SetEffect}, SkillData, SkillData, Optional{AudioClip}, Optional{string}, Optional{string})"/></term><description>Create and register a new <see cref="MySetEffectData"/>.</description></item>
    /// </list>
    /// </remarks>
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

        /// <summary>
        /// Register an existing <see cref="MySetEffectData"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="setEffect"/> will be assigned a new valid id.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MySetEffectManager"/> catalog has already loaded.</exception>
        /// <exception cref="ArgumentException"><paramref name="setEffect"/> is null.</exception>
        /// <param name="setEffect">The existing build to add.</param>
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

        /// <summary>
        /// Create and register a new <see cref="MySetEffectData"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="MySetEffectManager"/> catalog has already loaded.</exception>
        /// <param name="name">The name of this set effect.</param>
        /// <param name="icon">An icon for this set effect.</param>
        /// <param name="mainWeapon">A main hand weapon required to activate this set effect.</param>
        /// <param name="offWeapon">An off hand weapon required to activate this set effect.</param>
        /// <param name="accessories">Accessories required to activate this set effect.</param>
        /// <param name="effects">Status effects granted by this set effect.</param>
        /// <param name="prefab">A prefab for this set effect.</param>
        /// <param name="primarySkill">A primary skill granted by this set effect.</param>
        /// <param name="secondarySkill">A secondary skill granted by this set effect.</param>
        /// <param name="setEffectClip">An audio clip for activating this set effect. Defaults to <c>(seteffectSound)magic_shinny_high_tone_05</c>.</param>
        /// <param name="nameKey">A localization key for the name of this set effect. Will be auto-generated if left default.</param>
        /// <param name="descriptionKey">A localization key for the description of this set effect. Will be auto-generated if left default.</param>
        /// <returns>A new <see cref="MySetEffectData"/>.</returns>
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
