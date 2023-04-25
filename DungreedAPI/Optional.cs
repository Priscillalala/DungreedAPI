using System;
using UnityEngine;

namespace DungreedAPI
{
    public readonly struct Optional<T>
    {
		public Optional(T value)
		{
			this.value = value;
			Exists = true;
		}

		public bool Exists { get; }
		public T Value
		{
			get
			{
				if (!Exists)
				{
					throw new InvalidOperationException("InvalidOperation_NoRef");				
				}
				return value;
			}
		}

		public T GetRefOrDefault()
		{
			return value;
		}

		public T GetRefOrDefault(T defaultValue)
		{
			if (!Exists)
			{
				return defaultValue;
			}
			return value;
		}

		public override bool Equals(object other)
		{
			if (!Exists)
			{
				return other == null;
			}
			return other != null && value.Equals(other);
		}

		public override int GetHashCode()
		{
			if (!Exists)
			{
				return 0;
			}
			return value.GetHashCode();
		}

		public override string ToString()
		{
			if (!Exists)
			{
				return "";
			}
			return value.ToString();
		}

		public static implicit operator Optional<T>(T value)
		{
			return new Optional<T>(value);
		}

		public static explicit operator T(Optional<T> value)
		{
			return value.Value;
		}

		private readonly T value;
	}
}
