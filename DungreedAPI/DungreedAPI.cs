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
    public class DungreedAPI : BaseUnityPlugin
    {
        public static DungreedAPI instance { get; private set; }
        internal static ConfigFile config { get; private set; }
        internal static ManualLogSource logger { get; private set; }

        private void Awake()
        {
            instance = this;
            string configPath = Path.Combine(Paths.ConfigPath, "DungreedAPI.cfg");
            config = new ConfigFile(configPath, true, Info.Metadata);
            logger = Logger;

            CostumeAPI.Init();
            FoodAPI.Init();
            ItemAPI.Init();
            LocalizationAPI.Init();
            PrefabAPI.Init();
            SetEffectAPI.Init();

            MyAccessoryData accessory = ItemAPI.AddAccessory("TestAccessory", ItemRarityTier.UNCOMMON, 1000,
                icon: Resources.Load<MyItemData>("items/0240_RussianRoulette").icon,
                defense: 10,
                effects: new[] { "CRITICAL/20", "TOUGHNESS/-5" }
                );
            LocalizationAPI.Add(accessory.aName, "Test Accessory");
            LocalizationAPI.Add(accessory.aDescription, "This is a description...");

            MySetEffectData setEffect = SetEffectAPI.Add("TestSetEffect",
                icon: Resources.Load<MyItemData>("items/0240_RussianRoulette").icon,
                mainWeapon: Resources.Load<MyWeaponData>("items/0240_RussianRoulette"),
                accessories: new[] { accessory },
                effects: new[] { "PROTECTIONSHIELD/30" }
                );
            LocalizationAPI.Add(setEffect.aName, "Test Set Effect");
            LocalizationAPI.Add(setEffect.aDescription, "This is a description...For a Set Effect!");

            MyCostumeData costume = CostumeAPI.Add("Test Costome",
                icon: Resources.Load<MyItemData>("items/0240_RussianRoulette").icon,
                effects: new[] { "ATTACK_SPEED/100" }
                );
            LocalizationAPI.Add(costume.aName, "Test Costume");
            LocalizationAPI.Add(costume.aDescription, "This is a description...For a Costume!");
            //LocalizationAPI.AddXmlFile(Path.Combine(Path.GetDirectoryName(Info.Location), "testlanguage.xml"));
        }
    }
}
