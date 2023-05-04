namespace DungreedAPI
{
	public readonly record struct WeaponReload(WeaponReloadType reloadType, int maxShots, float reloadTime, float reloadDelayPerShot);
}
