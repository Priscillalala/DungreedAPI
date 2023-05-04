namespace DungreedAPI
{
	public readonly record struct WeaponAttack(WeaponAttackType attackType, Range<float> damage, float attackInterval, float spreadAngle, WeaponReload reload, AmmoSpeed ammoSpeed);
}
