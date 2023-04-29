using System;
using UnityEngine;

namespace DungreedAPI
{
    public readonly struct AbilityPerk
	{	
		public AbilityPerk(GameObject perk, Sprite inactive, Sprite active)
        {
			this.perk = perk;
			this.inactive = inactive;
			this.active = active;
		}

		public readonly GameObject perk;
		public readonly Sprite inactive;
		public readonly Sprite active;
	}
}
