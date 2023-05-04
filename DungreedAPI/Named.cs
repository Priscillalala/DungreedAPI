using System;
using UnityEngine;

namespace DungreedAPI
{
	/// <summary>
	/// Associates a <see cref="UnityEngine.Object"/> with a name to avoid repeated access of <see cref="UnityEngine.Object.name"/>.
	/// </summary>
	public readonly struct Named<T> where T : UnityEngine.Object
	{
		public Named(T value, string name)
		{
			this.value = value;
			this.name = name;
		}

		public T Value => value;
		public string Name => name;

		public override bool Equals(object other)
		{
			if (other is Named<T> named)
			{
				return named.name == name && named.value.Equals(value);
			}
			return value != null && value.Equals(other);
		}

		public override int GetHashCode()
		{
			return (value != null ? value.GetHashCode() : 0) ^ name.GetHashCode();
		}

		public override string ToString()
		{
			return value != null ? value.ToString() : string.Empty;
		}

		public static explicit operator T(Named<T> value)
		{
			return value.Value;
		}

		private readonly T value;
		private readonly string name;
	}
}
