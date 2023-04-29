namespace DungreedAPI
{
    public readonly struct WeaponReload
	{
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
