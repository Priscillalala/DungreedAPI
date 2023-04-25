using System;
using UnityEngine;

namespace DungreedAPI
{
    public readonly struct AmmoSpeed
    {
		public static AmmoSpeed Normal(float speed, bool allowRandomAcceleration = false) => 
			new AmmoSpeed(MyWeaponData.AmmoType.NORMAL, speed, 0, 0, allowRandomAcceleration);
		
		public static AmmoSpeed Acceleration(float fromSpeed, float toSpeed, bool allowRandomAcceleration = false) => 
			new AmmoSpeed(MyWeaponData.AmmoType.ACCELERATION, 0, fromSpeed, toSpeed, allowRandomAcceleration);
		
		public static AmmoSpeed Random(float minSpeed, float maxSpeed, bool allowRandomAcceleration = false) => 
			new AmmoSpeed(MyWeaponData.AmmoType.RANDOM_SPEED, 0, minSpeed, maxSpeed, allowRandomAcceleration);

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
