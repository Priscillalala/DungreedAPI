namespace DungreedAPI
{
    public readonly struct WeaponAttack
	{	
		public WeaponAttack(WeaponAttackType attackType, Range<float> damage, float attackInterval, float spreadAngle, WeaponReload reload, AmmoSpeed ammoSpeed)
        {
			this.attackType = attackType;
			this.damage = damage;
			this.attackInterval = attackInterval;
			this.spreadAngle = spreadAngle;
			this.reload = reload;
			this.ammoSpeed = ammoSpeed;
		}

		public readonly WeaponAttackType attackType;
		public readonly Range<float> damage;
		public readonly float attackInterval;
		public readonly float spreadAngle;
		public readonly WeaponReload reload;
		public readonly AmmoSpeed ammoSpeed;
	}
}
