using System;
using UnityEngine;

namespace DungreedAPI
{
    public readonly struct WeaponAttack
	{
		public static WeaponAttack Melee(float minDamage, float maxDamage, float attackInterval) => 
			new WeaponAttack(WeaponAttackType.MELEE, minDamage, maxDamage, attackInterval, 0, default, default);
		
		public static WeaponAttack Bullet(float minDamage, float maxDamage, float attackInterval, float spreadAngle, WeaponReload reload, AmmoSpeed ammoSpeed) => 
			new WeaponAttack(WeaponAttackType.BULLET, minDamage, maxDamage, attackInterval, spreadAngle, reload, ammoSpeed);
		
		public WeaponAttack(WeaponAttackType attackType, float minDamage, float maxDamage, float attackInterval, float spreadAngle, WeaponReload reload, AmmoSpeed ammoSpeed)
        {
			this.attackType = attackType;
			this.minDamage = minDamage;
			this.maxDamage = maxDamage;
			this.attackInterval = attackInterval;
			this.spreadAngle = spreadAngle;
			this.reload = reload;
			this.ammoSpeed = ammoSpeed;
		}

		public readonly WeaponAttackType attackType;
		public readonly float minDamage;
		public readonly float maxDamage;
		public readonly float attackInterval;
		public readonly float spreadAngle;
		public readonly WeaponReload reload;
		public readonly AmmoSpeed ammoSpeed;
	}
}
