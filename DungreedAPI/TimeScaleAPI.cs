using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using System.Xml;
using System.IO;

namespace DungreedAPI
{
    /// <summary>
    /// Set the time scale of the game in a non-destructive way..
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item><term><see cref="CreatePrefab(string, Type[])"/></term><description>Create a new <see cref="GameObject"/> prefab.</description></item>
    /// <item><term><see cref="ClonePrefab(GameObject, string)"/></term><description>Clone an existing <see cref="GameObject"/> prefab.</description></item>
    /// </list>
    /// </remarks>
    public static class TimeScaleAPI
    {
        internal static SortedSet<Handle> handles = new SortedSet<Handle>();

        internal static Handle gameManagerHandle;

        internal static void Init() 
        {
            On.GameManager.SetGameTimeScale += GameManager_SetGameTimeScale;
            On.GameManager.ResetGameTimeScale += GameManager_ResetGameTimeScale;
        }

        private static void GameManager_SetGameTimeScale(On.GameManager.orig_SetGameTimeScale orig, GameManager self, float timeScale)
        {
            if (!self.isTestTimeScaleActivated)
            {
                if (timeScale == 1f)
                {
                    UnsetGameManagerTimeScale();
                } 
                else
                {
                    SetGameManagerTimeScale(timeScale);
                }
            }
        }

        private static void GameManager_ResetGameTimeScale(On.GameManager.orig_ResetGameTimeScale orig, GameManager self)
        {
            if (!self.isTestTimeScaleActivated)
            {
                UnsetGameManagerTimeScale();
            }
        }

        internal static void SetGameManagerTimeScale(float timeScale)
        {
            if (gameManagerHandle == null)
            {
                DungreedAPI.logger.LogInfo("Set Game Manager time scale: " + timeScale);
                SetTimeScale(timeScale, out gameManagerHandle);
            }
        }

        internal static void UnsetGameManagerTimeScale()
        {
            if (gameManagerHandle != null)
            {
                DungreedAPI.logger.LogInfo("Unset game manager time scale");
                UnsetTimeScale(ref gameManagerHandle);
            }
        }

        internal static void RecalculateTimeScale()
        {
            float timeScale = handles.Count > 0 ? handles.Min.TimeScale : 1f;
            DungreedAPI.logger.LogInfo($"reclalc time scale. handles count is {handles.Count}, time scale is {timeScale}");
            if (GameManager.Instance)
            {
                GameManager.Instance.timeScale = timeScale;
                if (!GameManager.Instance.Paused)
                {
                    Time.timeScale = timeScale;
                }
            }
            else
            {
                Time.timeScale = timeScale;
            }
        }

        public static void SetTimeScale(float timeScale, out Handle handle)
        {
            handle = new Handle(timeScale);
            handles.Add(handle);
            RecalculateTimeScale();
        }

        public static bool UnsetTimeScale(ref Handle handle)
        {
            bool result = handles.Remove(handle);
            RecalculateTimeScale();
            handle = null;
            return result;
        }

        public class Handle : IComparable, IComparable<Handle>
        {
            internal Handle(float timeScale)
            {
                this.timeScale = timeScale;
            }

            public int CompareTo(object obj)
            {
                if (obj is Handle handle)
                {
                    return CompareTo(handle);
                }
                return 0;
            }

            public int CompareTo(Handle other)
            {
                if (other == null)
                {
                    return -1;
                }
                return timeScale.CompareTo(other.timeScale);
            }

            public float TimeScale => timeScale;

            private float timeScale;
        }
    }
}
