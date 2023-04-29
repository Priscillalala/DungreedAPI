namespace DungreedAPI
{
    public readonly struct AmmoSpeed
    {
		public AmmoSpeed(MyWeaponData.AmmoType ammoSpeedType, float speed, float minSpeed, float maxSpeed, bool allowRandomAcceleration)
        {
			this.ammoSpeedType = ammoSpeedType;
			this.speed = speed;
			this.minSpeed = minSpeed;
			this.maxSpeed = maxSpeed;
			this.allowRandomAcceleration = allowRandomAcceleration;
        }

		public readonly MyWeaponData.AmmoType ammoSpeedType;
		public readonly float speed;
		public readonly float minSpeed;
		public readonly float maxSpeed;
		public readonly bool allowRandomAcceleration;
	}
}
