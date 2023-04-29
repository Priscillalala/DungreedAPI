namespace DungreedAPI
{
    public static class AmmoSpeeds
	{
		public static AmmoSpeed Normal(float speed, bool allowRandomAcceleration = false) => 
			new AmmoSpeed(MyWeaponData.AmmoType.NORMAL, speed, 0, 0, allowRandomAcceleration);
		
		public static AmmoSpeed Acceleration(float fromSpeed, float toSpeed, bool allowRandomAcceleration = false) => 
			new AmmoSpeed(MyWeaponData.AmmoType.ACCELERATION, 0, fromSpeed, toSpeed, allowRandomAcceleration);
		
		public static AmmoSpeed Random(float minSpeed, float maxSpeed, bool allowRandomAcceleration = false) => 
			new AmmoSpeed(MyWeaponData.AmmoType.RANDOM_SPEED, 0, minSpeed, maxSpeed, allowRandomAcceleration);
	}
}
