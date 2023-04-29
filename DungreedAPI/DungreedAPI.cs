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
        //public static DungreedAPI instance { get; private set; }
        internal static ConfigFile config { get; private set; }
        internal static ManualLogSource logger { get; private set; }

        internal void Awake()
        {
            //instance = this;
            string configPath = Path.Combine(Paths.ConfigPath, "DungreedAPI.cfg");
            config = new ConfigFile(configPath, true, Info.Metadata);
            logger = Logger;

            AbilityAPI.Init();
            CostumeAPI.Init();
            FoodAPI.Init();
            ItemAPI.Init();
            LocalizationAPI.Init();
            PrefabAPI.Init();
            SetEffectAPI.Init();
            SkillAPI.Init();
            SoulPerkAPI.Init();

            MyAccessoryData accessory = ItemAPI.AddNewAccessory("TestAccessory", ItemRarityTier.UNCOMMON, 1000,
                icon: Resources.Load<MyItemData>("items/0240_RussianRoulette").icon,
                defense: 10,
                effects: new[] { Effects.CRITICAL(20), Effects.TOUGHNESS(-5) }
                );
            LocalizationAPI.Add(accessory.aName, "Test Accessory");
            LocalizationAPI.Add(accessory.aDescription, "This is a description...");

            MySetEffectData setEffect = SetEffectAPI.AddNew("TestSetEffect",
                icon: Resources.Load<MyItemData>("items/0240_RussianRoulette").icon,
                mainWeapon: Resources.Load<MyWeaponData>("items/0240_RussianRoulette"),
                accessories: new[] { accessory },
                effects: new[] { Effects.PROTECTIONSHIELD(30) }
                );
            LocalizationAPI.Add(setEffect.aName, "Test Set Effect");
            LocalizationAPI.Add(setEffect.aDescription, "This is a description...For a Set Effect!");

            MyCostumeData costume = CostumeAPI.AddNew("Test Costome",
                icon: Resources.Load<MyItemData>("items/0240_RussianRoulette").icon,
                effects: new[] { Effects.ATTACK_SPEED(20) }
                );
            LocalizationAPI.Add(costume.aName, "Test Costume");
            LocalizationAPI.Add(costume.aDescription, "This is a description...For a Costume!");
            //LocalizationAPI.AddXmlFile(Path.Combine(Path.GetDirectoryName(Info.Location), "testlanguage.xml"));
            AssetBundle bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Info.Location), "testdungreedassets"));
            
            MyFoodData food = FoodAPI.AddNew("Gingersnap", 1200, 35, new(3, 6), new(0, 10), icon: bundle.LoadAsset<Sprite>("Gingersnap"),
                foodType: MyFoodData.FoodType.SPECIALTY,
                effects: new[] { Effects.EVASION(20) },
                eatClip: Resources.Load<MyFoodData>("foods/VS110_ChocolateCookie").eatClip
                );
            LocalizationAPI.AddMany((food.aName, "Gingersnap"), (food.aDescription, "Delicious!"));

            Sprite testSprite = bundle.LoadAsset<Sprite>("Gingersnap");
            MyFullAbilityData ability = AbilityAPI.AddNew(
                name: "TestAbility",
                level5: new AbilityPerk(null, testSprite, testSprite),
                level10: new AbilityPerk(null, testSprite, testSprite),
                level20: new AbilityPerk(null, testSprite, testSprite),
                effects: new[] { Effects.PROTECTIONSHIELD(5) },
                background: testSprite
                );
            LocalizationAPI.AddMany((ability.aName, "Test Ability"), (ability.aDescription, "Ability description."));
        }
    }
}
