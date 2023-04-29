using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;

namespace DungreedAPI
{
    public static class SoulPerkAPI
    {
        internal static CatalogWrapper<MyPerkData> catalogWrapper;
        internal static List<Named<MySoulPerkData>> managedSoulPerks = new List<Named<MySoulPerkData>>();
        internal static ManagedSaveStringFormatter<Named<MySoulPerkData>> isUnlockedFormatter = new ManagedSaveStringFormatter<Named<MySoulPerkData>>("Perk_IsUnlocked_{0}", x => x.Value.id, x => x.Name);
        internal static Dictionary<Player, HashSet<string>> playerToActiveSoulPerks = new Dictionary<Player, HashSet<string>>();

        internal static void Init()
        {
            On.Player.ActivateSoulPerk += Player_ActivateSoulPerk;
            On.GameManager.DestroyPlayer += GameManager_DestroyPlayer;
            On.MySoulPerksManager.Initialize += MySoulPerksManager_Initialize;
            SaveInjector.beforeSave += SaveInjector_beforeSave;
        }

        private static void MySoulPerksManager_Initialize(On.MySoulPerksManager.orig_Initialize orig, MySoulPerksManager self)
        {
            orig(self);
            catalogWrapper = new CatalogWrapper<MyPerkData>(self.perks, 1);
            foreach (Named<MySoulPerkData> perk in managedSoulPerks)
            {
                if (perk.Value.disable)
                {
                    continue;
                }
                perk.Value.id = catalogWrapper.Add(perk.Value);
                if (TryUnlockManagedSoulPerk(perk))
                {
                    self.availablePerks.Add(perk.Value.id, perk.Value);
                }
            }
        }

        internal static bool TryUnlockManagedSoulPerk(Named<MySoulPerkData> soulPerk)
        {
            SaveData current = SaveManager.GetCurrent(); 
            if (!current.TryGetValue(isUnlockedFormatter.IdFormat(soulPerk), out bool unlocked) && current.TryGetValue(isUnlockedFormatter.NameFormat(soulPerk), out unlocked))
            {
                current[isUnlockedFormatter.IdFormat(soulPerk)] = unlocked;
                current.Remove(isUnlockedFormatter.NameFormat(soulPerk));
            }
            return unlocked;
        }

        private static void SaveInjector_beforeSave(Dictionary<string, SaveData.DataContainer> obj)
        {
            foreach (Named<MySoulPerkData> soulPerk in managedSoulPerks)
            {
                if (obj.TryGetValue(isUnlockedFormatter.IdFormat(soulPerk), out SaveData.DataContainer unlockedData))
                {
                    unlockedData.key = isUnlockedFormatter.NameFormat(soulPerk);
                }
            }
        }

        private static void Player_ActivateSoulPerk(On.Player.orig_ActivateSoulPerk orig, Player self, string key)
        {
            if (!playerToActiveSoulPerks.TryGetValue(self, out HashSet<string> keys))
            {
                keys = new HashSet<string>();
                playerToActiveSoulPerks.Add(self, keys);
            }
            keys.Add(key);
            orig(self, key);
        }

        private static void GameManager_DestroyPlayer(On.GameManager.orig_DestroyPlayer orig, GameManager self, bool with2P)
        {
            playerToActiveSoulPerks.Remove(self.currentPlayer);
            orig(self, with2P);
        }

        public static bool HasActiveSoulPerk(this Player player, MySoulPerkData soulPerk) => HasActiveSoulPerk(player, soulPerk.key);

        public static bool HasActiveSoulPerk(this Player player, string key) => playerToActiveSoulPerks.TryGetValue(player, out HashSet<string> keys) && keys.Contains(key);

        public static void AddExisting(MySoulPerkData soulPerk)
        {
            if (MySoulPerksManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            if (!soulPerk)
            {
                throw new ArgumentException();
            }
            managedSoulPerks.Add(new Named<MySoulPerkData>(soulPerk, soulPerk.name));
        }

        public static MySoulPerkData AddNew(string name, SoulShopUnlock unlock, Sprite icon,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default)
        {
            if (MySoulPerksManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            MySoulPerkData mySoulPerk = ScriptableObject.CreateInstance<MySoulPerkData>();
            mySoulPerk.name = name;
            mySoulPerk.key = name;
            mySoulPerk.icon = icon;
            mySoulPerk.aName = nameKey.Exists ? nameKey.Value : $"SoulPerk_{name}_Name";
            mySoulPerk.aDescription = descriptionKey.Exists ? descriptionKey.Value : $"SoulPerk_{name}_Description";
            mySoulPerk.disable = false;
            SoulShopUtil.AddSoulPerk(mySoulPerk, unlock);
            managedSoulPerks.Add(new Named<MySoulPerkData>(mySoulPerk, name));
            return mySoulPerk;
        }
    }
}
