namespace DungreedAPI
{
    public static class AmmoSpeeds
	{
		public static AmmoSpeed NORMAL(float speed, bool allowRandomAcceleration = false) => 
			new AmmoSpeed(MyWeaponData.AmmoType.NORMAL, new Range<float>(speed, speed), allowRandomAcceleration);
		
		public static AmmoSpeed ACCELERATION(float fromSpeed, float toSpeed, bool allowRandomAcceleration = false) => 
			new AmmoSpeed(MyWeaponData.AmmoType.ACCELERATION, new Range<float>(fromSpeed, toSpeed), allowRandomAcceleration);
		
		public static AmmoSpeed RANDOM_SPEED(Range<float> speed, bool allowRandomAcceleration = false) => 
			new AmmoSpeed(MyWeaponData.AmmoType.RANDOM_SPEED, speed, allowRandomAcceleration);
	}
}
