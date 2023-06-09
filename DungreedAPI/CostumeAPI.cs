﻿using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;

namespace DungreedAPI
{
    /// <summary>
    /// Create and register costumes for the boutique.
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item><term><see cref="Add(MyCostumeData)"/></term><description>Register an existing <see cref="MyCostumeData"/>.</description></item>
    /// <item><term><see cref="AddNew(string, GameObjectWithComponent{Char_Costume}, Sprite, bool, string[], MyItemData[], Sprite, Sprite, MyItemData[], bool, Optional{string}, Optional{string})"/></term><description>Create and register a new <see cref="MyCostumeData"/>.</description></item>
    /// </list>
    /// </remarks>
    public static class CostumeAPI
    {
        internal static CatalogWrapper<MyCostumeData> catalogWrapper;
        internal static List<Named<MyCostumeData>> managedCostumes = new List<Named<MyCostumeData>>();
        internal static ManagedSaveStringFormatter<Named<MyCostumeData>> isUnlockedFormatter = new ManagedSaveStringFormatter<Named<MyCostumeData>>("Costume_IsUnlocked_{0}", x => x.Value.id, x => x.Name);

        internal static void Init()
        {
            On.MyCostumesManager.Initialize += MyCostumesManager_Initialize;
            SaveInjector.beforeSave += SaveInjector_beforeSave;
        }

        private static void MyCostumesManager_Initialize(On.MyCostumesManager.orig_Initialize orig, MyCostumesManager self)
        {
            orig(self);
            catalogWrapper = new CatalogWrapper<MyCostumeData>(self.costumes, 18);
            foreach (Named<MyCostumeData> costume in managedCostumes)
            {
                if (costume.Value.disable)
                {
                    continue;
                }
                costume.Value.id = catalogWrapper.Add(costume.Value);
                if (TryUnlockManagedItem(costume))
                {
                    self.availableCostumes.Add(costume.Value.id, costume.Value);
                }
            }
        }

        internal static bool TryUnlockManagedItem(Named<MyCostumeData> costume)
        {
            SaveData current = SaveManager.GetCurrent();
            bool unlocked;
            if (costume.Value.basicItem)
            {
                unlocked = true;
            } 
            else if (!current.TryGetValue(isUnlockedFormatter.IdFormat(costume), out unlocked) && current.TryGetValue(isUnlockedFormatter.NameFormat(costume), out unlocked))
            {
                current[isUnlockedFormatter.IdFormat(costume)] = unlocked;
                current.Remove(isUnlockedFormatter.NameFormat(costume));
            }
            return unlocked;
        }

        private static void SaveInjector_beforeSave(Dictionary<string, SaveData.DataContainer> obj)
        {
            foreach (Named<MyCostumeData> costume in managedCostumes)
            {
                if (!costume.Value.basicItem && obj.TryGetValue(isUnlockedFormatter.IdFormat(costume), out SaveData.DataContainer unlockedData))
                {
                    unlockedData.key = isUnlockedFormatter.NameFormat(costume);
                }
            }
        }

        /// <summary>
        /// Register an existing <see cref="MyCostumeData"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="costume"/> will be assigned a new valid id.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MyCostumesManager"/> catalog has already loaded.</exception>
        /// <exception cref="ArgumentException"><paramref name="costume"/> is null.</exception>
        /// <param name="costume">The existing costume to add.</param>
        public static void Add(MyCostumeData costume)
        {
            if (MyCostumesManager.Instance.IsInitialized)
            {
                throw new InvalidOperationException();
            }
            if (!costume)
            {
                throw new ArgumentException();
            }
            managedCostumes.Add(new Named<MyCostumeData>(costume, costume.name));
        }

        /// <summary>
        /// Create and register a new <see cref="MyCostumeData"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="MyCostumesManager"/> catalog has already loaded.</exception>
        /// <param name="name">The name of this costume.</param>
        /// <param name="prefab">A prefab for this costume.</param>
        /// <param name="icon">An icon for this costume.</param>
        /// <param name="startsUnlocked">Should this costume start unlocked?</param>
        /// <param name="effects">Stattus effects granted by this costume.</param>
        /// <param name="basicEquipmentItems">Items granted to this costume.</param>
        /// <param name="handSprite">A sprite for the hands of this costume.</param>
        /// <param name="unlockIcon">An unlock icon for this costume.</param>
        /// <param name="unlockItemsAfterDefeatKaminela">Items unlocked by defeating Kaminela with this costume.</param>
        /// <param name="cantEquipWings">Should this costume be banned from equipping #Wings?</param>
        /// <param name="nameKey">A localization key for the name of this costume. Will be auto-generated if left default.</param>
        /// <param name="descriptionKey">A localization key for the description of this costume. Will be auto-generated if left default.</param>
        /// <returns>A new <see cref="MyCostumeData"/>.</returns>
        public static MyCostumeData AddNew(string name, GameObjectWithComponent<Char_Costume> prefab, Sprite icon,
            bool startsUnlocked = true,
            string[] effects = null,
            MyItemData[] basicEquipmentItems = null,
            Sprite handSprite = null,
            Sprite unlockIcon = null,
            MyItemData[] unlockItemsAfterDefeatKaminela = null,
            bool cantEquipWings = false,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default)
        {
            if (MyCostumesManager.Instance.IsInitialized)
            {
                throw new InvalidOperationException();
            }
            MyCostumeData costume = ScriptableObject.CreateInstance<MyCostumeData>();
            costume.name = name;
            costume.rarity = ItemRarityTier.COMMON;
            costume.price = 0;
            costume.basicItem = startsUnlocked;
            costume.sellAtShop = false;
            costume.visibleInList = true;
            costume.isSpecial = false;
            costume.appearInTownNPC = true;
            costume.universe = ItemUniverse.NONE;
            costume.tags = null;
            costume.icon = icon;
            costume.resourcePrefab = prefab;
            costume.cantAmplifyEffect = false;
            costume.altarBonus = 0;
            costume.allowAdditionalRandomOptions = false;
            costume.effects = effects ?? Array.Empty<string>();
            costume.randomEffects = Array.Empty<string>();
            costume.basicEquipmentItem = basicEquipmentItems ?? Array.Empty<MyItemData>();
            costume.handSprite = handSprite;
            costume.unlockIcon = unlockIcon;
            costume.unlockItemAfterDefeatKaminela = unlockItemsAfterDefeatKaminela ?? Array.Empty<MyItemData>();
            costume.wingsEquipRestriction = cantEquipWings;
            costume.aName = nameKey.Exists ? nameKey.Value : $"Costume_{name}_Name";
            costume.aDescription = descriptionKey.Exists ? descriptionKey.Value : $"Costume_{name}_Description";
            costume.level = 0;
            costume.nicalis = false;
            costume.disable = false;
            managedCostumes.Add(new Named<MyCostumeData>(costume, name));
            return costume;
        }
    }
}
