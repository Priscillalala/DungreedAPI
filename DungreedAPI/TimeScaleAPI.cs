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
    /// Alter the time scale of the game non-destructively.
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item><term><see cref="SetTimeScale(float, out Handle)"/></term><description>Set an ideal game time scale, starting a time scale operation.</description></item>
    /// <item><term><see cref="UnsetTimeScale(ref Handle)"/></term><description>Unset a time scale handle to complete a time scale operation.</description></item>
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

        /// <summary>
        /// Set an ideal game time scale, starting a time scale operation.
        /// </summary>
        /// <param name="timeScale">The ideal time scale.</param>
        /// <param name="handle">A time scale handle to represent this operation.</param>
        public static void SetTimeScale(float timeScale, out Handle handle)
        {
            handle = new Handle(timeScale);
            handles.Add(handle);
            RecalculateTimeScale();
        }

        /// <summary>
        /// Unset a time scale handle to complete a time scale operation.
        /// </summary>
        /// <remarks>
        /// <paramref name="handle"/> will be assigned to null.
        /// </remarks>
        /// <param name="handle">The time scale handle to unset.</param>
        /// <returns>true if the handle is successfully unset; otherwise, false.</returns>
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

            private readonly float timeScale;
        }
    }
}
