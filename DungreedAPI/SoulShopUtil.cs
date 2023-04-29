using System;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Linq;
using System.Collections.Generic;

namespace DungreedAPI
{
    internal static class SoulShopUtil
    {
        internal static NPC_ItemUnlocker soulShop
        {
            get
            {
                if (!_soulShop)
                {
                    _soulShop = Resources.Load<MyNPCData>("npc/24_ItemUnlocker")?.npcTownPrefab?.GetComponent<NPC_ItemUnlocker>();
                }
                return _soulShop;
            }
        }

        private static NPC_ItemUnlocker _soulShop;

        internal static List<SoulShopUnlockInfo> GetSlotList(SoulShopSlot slot)
        {
            return slot switch
            {
                SoulShopSlot.One => soulShop.itemsSlot1,
                SoulShopSlot.Two => soulShop.itemsSlot2,
                SoulShopSlot.Three => soulShop.itemsSlot3,
                _ => null,
            };
        }

        internal static void AddSoulPerk(MySoulPerkData soulPerk, SoulShopUnlock unlock)
        {
            List<SoulShopUnlockInfo> slotList = GetSlotList(unlock.slot);
            if (slotList == null)
            {
                return;
            }
            int index = slotList.FindLastIndex(x => x.type == SoulShopUnlockInfo.EType.Perk);
            SoulShopUnlockInfo unlockInfo = new SoulShopUnlockInfo();
            unlockInfo.cost = unlock.cost;
            unlockInfo.perk = soulPerk;
            unlockInfo.type = SoulShopUnlockInfo.EType.Perk;
            slotList.Insert(index + 1, unlockInfo);
        }

        internal static void AddItem(MyItemData item, SoulShopUnlock unlock)
        {
            List<SoulShopUnlockInfo> slotList = GetSlotList(unlock.slot);
            if (slotList == null)
            {
                return;
            }
            SoulShopUnlockInfo unlockInfo = new SoulShopUnlockInfo();
            unlockInfo.cost = unlock.cost;
            unlockInfo.item = item;
            unlockInfo.type = SoulShopUnlockInfo.EType.Item;
            slotList.Add(unlockInfo);
        }
    }
}
