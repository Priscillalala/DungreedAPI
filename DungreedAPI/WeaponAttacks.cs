namespace DungreedAPI
{
    public static class WeaponAttacks
	{
		public static WeaponAttack Melee(Range<float> damage, float attackInterval) => 
			new WeaponAttack(WeaponAttackType.MELEE, damage, attackInterval, 0, default, default);
		
		public static WeaponAttack Bullet(Range<float> damage, float attackInterval, float spreadAngle, WeaponReload reload, AmmoSpeed ammoSpeed) => 
			new WeaponAttack(WeaponAttackType.BULLET, damage, attackInterval, spreadAngle, reload, ammoSpeed);
	}
}
