using UnityEngine;

namespace DungreedAPI
{
	public readonly record struct AbilityPerk(GameObjectWithComponent<Player_Accessory> prefab, Sprite inactive, Sprite active);
}
