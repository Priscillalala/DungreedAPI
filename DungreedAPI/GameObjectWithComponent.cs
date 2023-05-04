using System;
using UnityEngine;

namespace DungreedAPI
{
	/// <summary>
	/// Represents a <see cref="GameObject"/> argument with a component of type <typeparamref name="T"/>.
	/// </summary>
	/// <remarks>
	/// Implicitly converts from a <see cref="GameObject"/> or a component of type <typeparamref name="T"/>.
	/// </remarks>
	public class GameObjectWithComponent<T> where T : MonoBehaviour
	{
		private GameObjectWithComponent(GameObject gameObject)
		{
			this.gameObject = gameObject;
			component = gameObject.GetComponent<T>();
			if (!component)
            {
				throw new ArgumentException();
            }
		}

		private GameObjectWithComponent(T component)
		{
			this.component = component;
			gameObject = component.gameObject;
		}

		public override bool Equals(object other)
		{
			return other is GameObjectWithComponent<T> prefabWith && gameObject == prefabWith.gameObject;
		}

		public override int GetHashCode()
		{
			return gameObject.GetHashCode();
		}

		public override string ToString()
		{
			return gameObject.ToString();
		}

		public static implicit operator GameObject(GameObjectWithComponent<T> value) => value?.gameObject;

		public static implicit operator T(GameObjectWithComponent<T> value) => value?.component;

		public static implicit operator GameObjectWithComponent<T>(T value) => new GameObjectWithComponent<T>(value);

		public static implicit operator GameObjectWithComponent<T>(GameObject value) => new GameObjectWithComponent<T>(value);


		private readonly GameObject gameObject;
		private readonly T component;
	}
}
