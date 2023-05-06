using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Security;

namespace DungreedAPI
{
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

        public static TSkillData NewSkill<TSkillData>(string name, float cooldown, Sprite icon,
            Action<TSkillData> setupSkill = null,
            AudioClip castClip = null,
            bool useOnAttackAnimation = false,
            string castAnimationTrigger = "",
            bool dontUseBasicRotation = false,
            float defaultRotation = 0f,
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
            skill.dontUseBasicRotation = dontUseBasicRotation;
            skill.defaultRotation = defaultRotation;
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
