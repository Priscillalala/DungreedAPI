using System;
using UnityEngine;

namespace DungreedAPI
{
    public readonly struct SoulShopUnlock
	{
		public SoulShopUnlock(int cost, SoulShopSlot slot)
        {
			this.cost = cost;
			this.slot = slot;
		}

		public readonly int cost;
		public readonly SoulShopSlot slot;
	}
}
