namespace DungreedAPI
{
	public readonly record struct AmmoSpeed(MyWeaponData.AmmoType ammoSpeedType, Range<float> speed, bool allowRandomAcceleration);
}
