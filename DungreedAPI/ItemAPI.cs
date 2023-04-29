using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;

namespace DungreedAPI
{
    public static class ItemAPI
    {
        internal static CatalogWrapper<MyItemData> catalogWrapper;
        internal static List<Named<MyItemData>> managedItems = new List<Named<MyItemData>>();
        internal static ManagedSaveStringFormatter<Named<MyItemData>> isUnlockedFormatter = new ManagedSaveStringFormatter<Named<MyItemData>>("Item_IsUnlocked_{0}", x => x.Value.id, x => x.Name);

        internal static void Init()
        {
            On.MyItemManager.Initialize += MyItemManager_Initialize; ;
            SaveInjector.beforeSave += SaveInjector_beforeSave;
        }

        private static void MyItemManager_Initialize(On.MyItemManager.orig_Initialize orig, MyItemManager self)
        {
            orig(self);
            catalogWrapper = new CatalogWrapper<MyItemData>(self.items, 323);
            foreach (Named<MyItemData> item in managedItems)
            {
                if (item.Value.disable)
                {
                    continue;
                }
                item.Value.id = catalogWrapper.Add(item.Value);
                self.availableItemsByRarity[item.Value.rarity][item.Value.id] = item.Value;
                if (TryUnlockManagedItem(item))
                {
                    self.availableItems.Add(item.Value.id, item.Value);
                    self.availableItemsByRarity[item.Value.rarity][item.Value.id] = item.Value;
                }
            }
        }

        internal static bool TryUnlockManagedItem(Named<MyItemData> item)
        {
            SaveData current = SaveManager.GetCurrent();
            bool unlocked;
            if (item.Value.basicItem)
            {
                unlocked = true;
            } 
            else if (!current.TryGetValue(isUnlockedFormatter.IdFormat(item), out unlocked) && current.TryGetValue(isUnlockedFormatter.NameFormat(item), out unlocked))
            {
                current[isUnlockedFormatter.IdFormat(item)] = unlocked;
                current.Remove(isUnlockedFormatter.NameFormat(item));
            }
            return unlocked;
        }

        private static void SaveInjector_beforeSave(Dictionary<string, SaveData.DataContainer> obj)
        {
            foreach (Named<MyItemData> item in managedItems)
            {
                if (!item.Value.basicItem && obj.TryGetValue(isUnlockedFormatter.IdFormat(item), out SaveData.DataContainer unlockedData))
                {
                    unlockedData.key = isUnlockedFormatter.NameFormat(item);
                }
            }
        }

        public static void AddExisting(MyItemData item)
        {
            if (MyItemManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            if (!item)
            {
                throw new ArgumentException();
            }
            managedItems.Add(new Named<MyItemData>(item, item.name));
        }

        public static MyAccessoryData AddNewAccessory(string name, ItemRarityTier rarity, int price, Sprite icon,
            bool startsUnlocked = true,
            bool sellAtShop = true,
            bool visibleInList = true,
            bool isSpecial = false,
            bool appearInTownNPC = true,
            ItemUniverse universe = ItemUniverse.NONE,
            string tags = null,
            GameObject resourcePrefab = null,
            bool cantAmplifyEffect = false,
            float altarBonus = 0,
            bool allowAdditionalRandomOptions = true,
            string[] effects = null,
            string[] randomEffects = null,
            int defense = 0,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default)
        {
            if (MyItemManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            MyAccessoryData accessory = ScriptableObject.CreateInstance<MyAccessoryData>();
            accessory.name = name;
            accessory.rarity = rarity;
            accessory.price = price;
            accessory.basicItem = startsUnlocked;
            accessory.sellAtShop = sellAtShop;
            accessory.visibleInList = visibleInList;
            accessory.isSpecial = isSpecial;
            accessory.appearInTownNPC = appearInTownNPC;
            accessory.universe = universe;
            accessory.tags = tags;
            accessory.icon = icon;
            accessory.resourcePrefab = resourcePrefab;
            accessory.cantAmplifyEffect = cantAmplifyEffect;
            accessory.altarBonus = altarBonus;
            accessory.allowAdditionalRandomOptions = allowAdditionalRandomOptions;
            accessory.effects = effects ?? Array.Empty<string>();
            accessory.randomEffects = randomEffects ?? Array.Empty<string>();
            accessory.defense = defense;
            accessory.aName = nameKey.Exists ? nameKey.Value : $"Item_{name}_Name";
            accessory.aDescription = descriptionKey.Exists ? descriptionKey.Value : $"Item_{name}_Description";
            accessory.level = 0;
            accessory.nicalis = false;
            accessory.disable = false;
            managedItems.Add(new Named<MyItemData>(accessory, name));
            return accessory;
        }

        public static MyWeaponData AddNewWeapon(string name, ItemRarityTier rarity, int price, WeaponAttack attack, WeaponHandType handType, WeaponInputType inputType, Sprite icon,
            bool startsUnlocked = true,
            bool sellAtShop = true,
            bool visibleInList = true,
            bool isSpecial = false,
            bool appearInTownNPC = true,
            ItemUniverse universe = ItemUniverse.NONE,
            string tags = null,
            GameObject resourcePrefab = null,
            GameObject additionalResourcePrefab = null,
            bool cantAmplifyEffect = false,
            float altarBonus = 0,
            bool allowAdditionalRandomOptions = true,
            string[] effects = null,
            string[] randomEffects = null,
            int defense = 0,
            SkillData primarySkill = null,
            SkillData secondarySkill = null,
            Vector2 spriteOffset = default,
            float attackSpeedMultiplier = 1f,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default)
        {
            if (MyItemManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            MyWeaponData weapon = ScriptableObject.CreateInstance<MyWeaponData>();
            weapon.name = name;
            weapon.rarity = rarity;
            weapon.price = price;
            weapon.basicItem = startsUnlocked;
            weapon.sellAtShop = sellAtShop;
            weapon.visibleInList = visibleInList;
            weapon.isSpecial = isSpecial;
            weapon.appearInTownNPC = appearInTownNPC;
            weapon.universe = universe;
            weapon.tags = tags;
            weapon.icon = icon;
            weapon.resourcePrefab = resourcePrefab;
            weapon.cantAmplifyEffect = cantAmplifyEffect;
            weapon.altarBonus = altarBonus;
            weapon.allowAdditionalRandomOptions = allowAdditionalRandomOptions;
            weapon.effects = effects ?? Array.Empty<string>();
            weapon.randomEffects = randomEffects ?? Array.Empty<string>();
            weapon.defense = defense;
            weapon.aName = nameKey.Exists ? nameKey.Value : $"Item_{name}_Name";
            weapon.aDescription = descriptionKey.Exists ? descriptionKey.Value : $"Item_{name}_Description";
            weapon.additionalResourcePrefab = additionalResourcePrefab;
            weapon.damage = attack.damage.min;
            weapon.maxDamage = attack.damage.max;
            weapon.reloadType = attack.reload.reloadType;
            weapon.attackType = attack.attackType;
            weapon.inputType = inputType;
            weapon.handType = handType;
            weapon.attackIntervalTime = attack.attackInterval;
            weapon.maxShots = attack.reload.maxShots;
            weapon.reloadTime = attack.reload.reloadTime;
            weapon.reloadDelayOneShot = attack.reload.reloadDelayPerShot;
            weapon.attackError = attack.spreadAngle;
            weapon.ammoSpeed = attack.ammoSpeed.speed;
            weapon.ammoSpeedType = attack.ammoSpeed.ammoSpeedType;
            weapon.ammoSpeedFrom = attack.ammoSpeed.minSpeed;
            weapon.ammoSpeedTo = attack.ammoSpeed.maxSpeed;
            weapon.allowRandomAccSpeed = attack.ammoSpeed.allowRandomAcceleration;
            weapon.level = 0;
            weapon.nicalis = false;
            weapon.disable = false;
            weapon.skillQ = primarySkill;
            weapon.skillE = secondarySkill;
            weapon.spriteOffset = spriteOffset;
            weapon.attackSpeedMultiplyer = attackSpeedMultiplier;
            managedItems.Add(new Named<MyItemData>(weapon, name));
            return weapon;
        }

        public static void AddItemToSoulShop(MyItemData item, SoulShopUnlock unlock) => SoulShopUtil.AddItem(item, unlock);
    }
}
