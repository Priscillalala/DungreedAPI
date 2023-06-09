﻿using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;

namespace DungreedAPI
{
    /// <summary>
    /// Create and register food for the food shop or dessert shop.
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item><term><see cref="Add(MyFoodData)"/></term><description>Register an existing <see cref="MyFoodData"/>.</description></item>
    /// <item><term><see cref="AddNew(string, int, int, Range{int}, Range{float}, Sprite, MyFoodData.FoodType, bool, string[], bool, GameObjectWithComponent{FoodObject}, AudioClip, Optional{string}, Optional{string})"/></term><description>Create and register a new <see cref="MyFoodData"/>.</description></item>
    /// </list>
    /// </remarks>
    public static class FoodAPI
    {
        internal static CatalogWrapper<MyFoodData> catalogWrapper;
        internal static List<Named<MyFoodData>> managedFoods = new List<Named<MyFoodData>>();
        internal static ManagedSaveStringFormatter<Named<MyFoodData>> isUnlockedFormatter = new ManagedSaveStringFormatter<Named<MyFoodData>>("Food_IsUnlocked_{0}", x => x.Value.id, x => x.Name);
        internal static ManagedSaveStringFormatter<Named<MyFoodData>> isEatenFormatter = new ManagedSaveStringFormatter<Named<MyFoodData>>("Food_IsEated_{0}", x => x.Value.id, x => x.Name);

        internal static void Init()
        {
            On.MyFoodManager.Initialize += MyFoodManager_Initialize;
            SaveInjector.beforeSave += SaveInjector_beforeSave;
        }

        private static void MyFoodManager_Initialize(On.MyFoodManager.orig_Initialize orig, MyFoodManager self)
        {
            orig(self);
            catalogWrapper = new CatalogWrapper<MyFoodData>(self.foods, 136);
            foreach (Named<MyFoodData> food in managedFoods)
            {
                if (food.Value.inactivated)
                {
                    continue;
                }
                food.Value.id = catalogWrapper.Add(food.Value);
                if (TryUnlockManagedFood(food))
                {
                    self.availableFoods.Add(food.Value.id, food.Value);
                }
            }
        }

        internal static bool TryUnlockManagedFood(Named<MyFoodData> food)
        {
            SaveData current = SaveManager.GetCurrent();
            bool unlocked;
            if (food.Value.isBasic)
            {
                unlocked = true;
            } 
            else if (!current.TryGetValue(isUnlockedFormatter.IdFormat(food), out unlocked) && current.TryGetValue(isUnlockedFormatter.NameFormat(food), out unlocked))
            {
                current[isUnlockedFormatter.IdFormat(food)] = unlocked;
                current.Remove(isUnlockedFormatter.NameFormat(food));
            }
            if (!current.HasKey(isEatenFormatter.IdFormat(food)) && current.TryGetValue(isEatenFormatter.NameFormat(food), out bool eaten))
            {
                current[isEatenFormatter.IdFormat(food)] = eaten;
                current.Remove(isEatenFormatter.NameFormat(food));
            }
            return unlocked;
        }

        private static void SaveInjector_beforeSave(Dictionary<string, SaveData.DataContainer> obj)
        {
            foreach (Named<MyFoodData> food in managedFoods)
            {
                if (!food.Value.isBasic && obj.TryGetValue(isUnlockedFormatter.IdFormat(food), out SaveData.DataContainer unlockedData))
                {
                    unlockedData.key = isUnlockedFormatter.NameFormat(food);
                }
                if (!obj.TryGetValue(isEatenFormatter.IdFormat(food), out SaveData.DataContainer eatenData))
                {
                    eatenData.key = isEatenFormatter.NameFormat(food);
                }
            }
        }

        /// <summary>
        /// Register an existing <see cref="MyFoodData"/>.
        /// </summary>
        /// <remarks>
        /// <paramref name="food"/> will be assigned a new valid id.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MyFoodManager"/> catalog has already loaded.</exception>
        /// <exception cref="ArgumentException"><paramref name="food"/> is null.</exception>
        /// <param name="food">The existing food to add.</param>
        public static void Add(MyFoodData food)
        {
            if (MyFoodManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            if (!food)
            {
                throw new ArgumentException();
            }
            managedFoods.Add(new Named<MyFoodData>(food, food.name));
        }

        /// <summary>
        /// Create and register a new <see cref="MyFoodData"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="MyFoodManager"/> catalog has already loaded.</exception>
        /// <param name="name">The name of this food.</param>
        /// <param name="basePrice">The base price of this food.</param>
        /// <param name="satiety">The satiety gained from eating this food.</param>
        /// <param name="randomHealing">A range of valid healing values for this food.</param>
        /// <param name="randomPower">A range of valid power values for this food.</param>
        /// <param name="icon">An icon for this food.</param>
        /// <param name="foodType">The food type of this food.</param>
        /// <param name="startsUnlocked">Should this food start unlocked?</param>
        /// <param name="effects">Status effects gained by eating this food.</param>
        /// <param name="canAmplifyEffects">Can the effects of this food be randomly amplified?</param>
        /// <param name="prefab">A prefab associated with this food.</param>
        /// <param name="eatClip">An audio clip for eating this food.</param>
        /// <param name="nameKey">A localization key for the name of this food. Will be auto-generated if left default.</param>
        /// <param name="descriptionKey">A localization key for the description of this food. Will be auto-generated if left default.</param>
        /// <returns>A new <see cref="MyFoodData"/>.</returns>
        public static MyFoodData AddNew(string name, int basePrice, int satiety, Range<int> randomHealing, Range<float> randomPower, Sprite icon,
            MyFoodData.FoodType foodType = MyFoodData.FoodType.NORMAL, 
            bool startsUnlocked = true,
            string[] effects = null,
            bool canAmplifyEffects = true,
            GameObjectWithComponent<FoodObject> prefab = null,
            AudioClip eatClip = null,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default)
        {
            if (MyFoodManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            MyFoodData food = ScriptableObject.CreateInstance<MyFoodData>();
            food.name = name;
            food.basePrice = basePrice;
            food.satiety = satiety;
            food.randomHealMin = randomHealing.min;
            food.randomHealMax = randomHealing.max;
            food.randomPowerMin = randomPower.min;
            food.randomPowerMax = randomPower.max;
            food.foodType = foodType;
            food.isBasic = startsUnlocked;
            food.cannotEatMoreTwo = false;
            food.icon = icon;
            food.resourcePrefab = prefab;
            food.canAmplify = canAmplifyEffects;
            food.buffEffects = effects ?? Array.Empty<string>();
            food.eatClip = eatClip;
            food.aName = nameKey.Exists ? nameKey.Value : $"Food_{name}_Name";
            food.aDescription = descriptionKey.Exists ? descriptionKey.Value : $"Food_{name}_Description";
            food.inactivated = false;
            managedFoods.Add(new Named<MyFoodData>(food, name));
            return food;
        }
    }
}
