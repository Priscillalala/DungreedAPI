using System;
using UnityEngine;

namespace DungreedAPI
{
	public readonly record struct SoulShopUnlock(int cost, SoulShopSlot slot);
}
