using System;
using UnityEngine;

namespace DungreedAPI
{
	public readonly struct Range<T> where T : IComparable<T>
	{
		public Range(T min, T max)
		{
			if (min.CompareTo(max) > 0)
            {
				throw new ArgumentException();
            }
			this.min = min;
			this.max = max;
		}

		public override bool Equals(object other)
		{
			return other is Range<T> range && min.Equals(range.min) && max.Equals(range.max);
		}

		public override int GetHashCode()
		{
			return (min != null ? min.GetHashCode() : 0) ^ (max != null ? max.GetHashCode() : 0);
		}

		public override string ToString()
		{
			return $"({(min != null ? min.ToString() : "null")} to {(max != null ? max.ToString() : "null")})";
		}

		public readonly T min;
		public readonly T max;
	}
}
