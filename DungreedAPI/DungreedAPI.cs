using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;
using System.IO;
using BepInEx.Configuration;
using BepInEx.Logging;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace DungreedAPI
{
    [BepInPlugin("com.groovesalad.DungreedAPI", "DungreedAPI", "1.0.0")]
    internal class DungreedAPI : BaseUnityPlugin
    {
        internal static ConfigFile config { get; private set; }
        internal static ManualLogSource logger { get; private set; }

        internal void Awake()
        {
            string configPath = Path.Combine(Paths.ConfigPath, "DungreedAPI.cfg");
            config = new ConfigFile(configPath, true, Info.Metadata);
            logger = Logger;

            SaveInjector.Init();
            AbilityPathAPI.Init();
            BuildAPI.Init();
            CostumeAPI.Init();
            FoodAPI.Init();
            ItemAPI.Init();
            LocalizationAPI.Init();
            NpcAPI.Init();
            PostProcessingAPI.Init();
            PrefabAPI.Init();
            SetEffectAPI.Init();
            SkillAPI.Init();
            SoulPerkAPI.Init();
            TimeScaleAPI.Init();
        }

        internal void LateUpdate()
        {
            if (PostProcessingAPI.shouldUpdate)
            {
                PostProcessingAPI.shouldUpdate = false;
                PostProcessingAPI.Update(false);
            }
        }
    }
}
