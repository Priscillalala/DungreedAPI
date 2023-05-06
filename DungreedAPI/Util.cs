using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DungreedAPI
{
    public static class Util
	{
		public static string GetRelativePath(this BaseUnityPlugin plugin, string relativePath) 
		{ 
			return Path.Combine(Path.GetDirectoryName(plugin.Info.Location), relativePath); 
		}

		public static void AddComponent<T>(this GameObject gameObject, out T instance) where T : Component
		{
			instance = gameObject.AddComponent<T>();
		}

        public static bool TryFind(this Transform transform, string n, out Transform child)
        {
            return child = transform.Find(n);
        }

        public static IEnumerable<Transform> AllChildren(this Transform transform)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                yield return transform.GetChild(i);
            }
        }

        /*public static void CopyTo<T>(this T src, T dest, bool copyFields = true, bool copyProperties = false) where T : ScriptableObject
        {
            if (copyFields)
            {
                FieldInfo[] fields = typeof(T).GetFields();
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo fieldInfo = fields[i];
                    fieldInfo.SetValue(dest, fieldInfo.GetValue(src));
                }
            }
            if (copyProperties)
            {
                PropertyInfo[] properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo propertyInfo = properties[i];
                    if (propertyInfo.CanRead && propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(dest, propertyInfo.GetValue(src));
                    }
                }
            }
        }*/

        public static void AddToDelegate<T>(ref T source, T value) where T : Delegate
        {
            source = (T)Delegate.Combine(source, value);
        }

        public static void RemoveFromDelegate<T>(ref T source, T value) where T : Delegate
        {
            source = (T)Delegate.Remove(source, value);
        }

        public static void AppendArray<T>(ref T[] array, T element)
        {
            int length = array.Length;
            Array.Resize(ref array, length + 1);
            array[length] = element;
        }
    }
}
