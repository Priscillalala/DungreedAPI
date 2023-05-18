using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;

namespace DungreedAPI
{
#pragma warning disable CS1584
    /// <summary>
    /// Create skills and register dog skills.
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item><term><see cref="AddDogSkill(SkillData)"/></term><description>Register an existing <see cref="SkillData"/> as a dog skill.</description></item>
    /// <item><term><see cref="NewSkill{TSkillData}(string, float, Sprite, Action{TSkillData}, AudioClip, bool, string, float?, GameObject, bool, Optional{string}, Optional{string}))"/></term><description>Create a new TSkillData.</description></item>
    /// </list>
    /// </remarks>
#pragma warning restore CS1584
    public static class SkillAPI
    {
        internal static CatalogWrapper<SkillData> dogCatalogWrapper;
        internal static List<Named<SkillData>> managedDogSkills = new List<Named<SkillData>>();

        internal static void Init()
        {
            On.MyDogSkillManager.Initialize += MyDogSkillManager_Initialize;
        }

        private static void MyDogSkillManager_Initialize(On.MyDogSkillManager.orig_Initialize orig, MyDogSkillManager self)
        {
            orig(self);
            dogCatalogWrapper = new CatalogWrapper<SkillData>(self.skills, 6);
            foreach (Named<SkillData> dogSkill in managedDogSkills)
            {
                dogSkill.Value.id = dogCatalogWrapper.Add(dogSkill.Value);
            }
        }

        /// <summary>
        /// Register an existing <see cref="SkillData"/> as a dog skill.
        /// </summary>
        /// <remarks>
        /// <paramref name="dogSkill"/> will be assigned a new valid id.
        /// </remarks>
        /// <exception cref="InvalidOperationException">The <see cref="MyDogSkillManager"/> catalog has already loaded.</exception>
        /// <exception cref="ArgumentException"><paramref name="dogSkill"/> is null.</exception>
        /// <param name="dogSkill">The existing build to add.</param>
        public static void AddDogSkill(SkillData dogSkill)
        {
            if (MyDogSkillManager.Instance.LoadEnd)
            {
                throw new InvalidOperationException();
            }
            if (!dogSkill)
            {
                throw new ArgumentException();
            }
            managedDogSkills.Add(new Named<SkillData>(dogSkill, dogSkill.name));
        }

        /// <summary>
        /// Create a new <typeparamref name="TSkillData"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The <see cref="BuildManager"/> catalog has already loaded.</exception>
        /// <typeparam name="TSkillData"></typeparam>
        /// <param name="name">The name of this skill.</param>
        /// <param name="cooldown">The cooldown of this skill.</param>
        /// <param name="icon">An icon for this skill.</param>
        /// <param name="setupSkill">Assign additional fields on <typeparamref name="TSkillData"/>.</param>
        /// <param name="castClip">An audio clip for activating this skill.</param>
        /// <param name="useOnAttackAnimation">Should this skill wait for an attack animation to activate?</param>
        /// <param name="castAnimationTrigger">An animation trigger when casting this skill.</param>
        /// <param name="defaultRotation">Should this skill assign a custom animation rotation on cast? What is the default rotation?</param>
        /// <param name="castFxPrefab">A visual prefab for casting this skill.</param>
        /// <param name="bobCameraOnCast">Should this skill bob the camera on cast?</param>
        /// <param name="nameKey">A localization key for the name of this skill. Will be auto-generated if left default.</param>
        /// <param name="descriptionKey">A localization key for the description of this skill. Will be auto-generated if left default.</param>
        /// <returns>A new <typeparamref name="TSkillData"/>.</returns>
        public static TSkillData NewSkill<TSkillData>(string name, float cooldown, Sprite icon,
            Action<TSkillData> setupSkill = null,
            AudioClip castClip = null,
            bool useOnAttackAnimation = false,
            string castAnimationTrigger = "",
            float? defaultRotation = null,
            GameObject castFxPrefab = null,
            bool bobCameraOnCast = false,
            Optional<string> nameKey = default,
            Optional<string> descriptionKey = default) where TSkillData : SkillData
        {
            TSkillData skill = ScriptableObject.CreateInstance<TSkillData>();
            skill.name = name;
            skill.cooldownTime = cooldown;
            skill.skillIcon = icon;
            skill.castClip = castClip;
            skill.useOnAttackAnimation = useOnAttackAnimation;
            skill.castAnimationTrigger = castAnimationTrigger;
            skill.dontUseBasicRotation = defaultRotation != null;
            skill.defaultRotation = defaultRotation ?? 0f;
            skill.castFxPrefab = castFxPrefab;
            skill.bobCameraOnCast = bobCameraOnCast;
            skill.skillName = nameKey.Exists ? nameKey.Value : $"Skill_{name}_Name";
            skill.skillDescription = descriptionKey.Exists ? descriptionKey.Value : $"Skill_{name}_Description";
            skill.publicSkill = false;
            skill.id = 0;
            setupSkill?.Invoke(skill);
            return skill;
        }
    }
}
