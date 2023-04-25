using System;
using UnityEngine;

namespace DungreedAPI
{
    public readonly struct WeaponReload
	{
		public static WeaponReload AllAtOnce(int maxShots, float reloadTime) => 
			new WeaponReload(WeaponReloadType.RELOAD_ALL, maxShots, reloadTime, 0);

		public static WeaponReload OneByOne(int maxShots, float reloadTime, float reloadDelayPerShot) =>
			new WeaponReload(WeaponReloadType.RELOAD_ONESHOT, maxShots, reloadTime, reloadDelayPerShot);

		public static WeaponReload Continuous(int maxShots, float reloadTime) => 
			new WeaponReload(WeaponReloadType.AUTO_FILL, maxShots, reloadTime, 0);
		public static WeaponReload None() =>
			new WeaponReload(WeaponReloadType.RELOAD_ALL, 0, 0, 0);

		public WeaponReload(WeaponReloadType reloadType, int maxShots, float reloadTime, float reloadDelayPerShot)
        {
			this.reloadType = reloadType;
			this.maxShots = maxShots;
			this.reloadTime = reloadTime;
			this.reloadDelayPerShot = reloadDelayPerShot;
        }

		public readonly WeaponReloadType reloadType;
		public readonly int maxShots;
		public readonly float reloadTime;
		public readonly float reloadDelayPerShot;
	}
}
