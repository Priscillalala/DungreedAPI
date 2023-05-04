using System;
using UnityEngine;

namespace DungreedAPI
{
	/// <summary>
	/// Represents an optional argument that can be passed as default to access a default implementation.
	/// </summary>
	/// <remarks>
	/// Implicitly converts from value of type <typeparamref name="T"/>.
	/// </remarks>
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

		public T GetValueOrDefault()
		{
			return Exists ? value : default;
		}

		public T GetValueOrDefault(T defaultValue)
		{
			return Exists ? value : defaultValue;
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
