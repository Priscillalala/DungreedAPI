﻿namespace DungreedAPI
{
    public static class WeaponReloads
	{
		public static WeaponReload RELOAD_ALL(int maxShots, float reloadTime) => 
			new WeaponReload(WeaponReloadType.RELOAD_ALL, maxShots, reloadTime, 0);

		public static WeaponReload RELOAD_ONESHOT(int maxShots, float reloadTime, float reloadDelayPerShot) =>
			new WeaponReload(WeaponReloadType.RELOAD_ONESHOT, maxShots, reloadTime, reloadDelayPerShot);

		public static WeaponReload AUTO_FILL(int maxShots, float reloadTime) => 
			new WeaponReload(WeaponReloadType.AUTO_FILL, maxShots, reloadTime, 0);
		public static WeaponReload NONE() =>
			new WeaponReload(WeaponReloadType.RELOAD_ALL, 0, 0, 0);
	}
}
