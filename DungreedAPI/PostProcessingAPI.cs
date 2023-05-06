using System;
using BepInEx;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using System.Xml;
using System.IO;
using UnityEngine.PostProcessing;

namespace DungreedAPI
{
    public static class PostProcessingAPI
    {
        internal static int nextIdx = 2;
        internal static Dictionary<int, PostProcessingProfile> managedPostProcessing = new Dictionary<int, PostProcessingProfile>();
        internal static void Init()
        {
            On.GameCamera.Awake += GameCamera_Awake;
            On.GameCamera.SetPostEffectProfile += GameCamera_SetPostEffectProfile;
        }

        private static void GameCamera_Awake(On.GameCamera.orig_Awake orig, GameCamera self)
        {
            if (self.profiles != null && managedPostProcessing.Count > 0)
            {
                int max = managedPostProcessing.Keys.Max();
                if (self.profiles.Length <= max)
                {
                    Array.Resize(ref self.profiles, max + 1);
                }
                foreach (var pair in managedPostProcessing)
                {
                    self.profiles[pair.Key] = pair.Value;
                }
            }
            orig(self);
        }

        public static void Add(PostProcessingProfile postProcessing, out int idx)
        {
            idx = nextIdx++;
            managedPostProcessing.Add(idx, postProcessing);
            if (GameCamera.Instance)
            {
                if (GameCamera.Instance.profiles.Length <= idx)
                {
                    Array.Resize(ref GameCamera.Instance.profiles, idx + 1);
                }
                GameCamera.Instance.profiles[idx] = postProcessing;
            }
        }

        internal static PostProcessingProfile originalPostProcessing;
        internal static PostProcessingProfile postProcessing;
        internal static event Action<PostProcessingProfile> onUpdatePostProcessing;
        internal static bool shouldUpdate;

        private static void GameCamera_SetPostEffectProfile(On.GameCamera.orig_SetPostEffectProfile orig, GameCamera self, int idx)
        {
            orig(self, idx);
            if (self._postProcessingBehaviour?.profile)
            {
                originalPostProcessing = UnityEngine.Object.Instantiate(postProcessing = self._postProcessingBehaviour.profile);
                Update(false);
            }
        }

        internal static void Update(bool force)
        {
            if (!originalPostProcessing || !postProcessing || (!force && onUpdatePostProcessing == null))
            {
                return;
            }
            ResetModel(x => x.ambientOcclusion, x => ref x.m_Settings);
            ResetModel(x => x.antialiasing, x => ref x.m_Settings);
            ResetModel(x => x.bloom, x => ref x.m_Settings);
            ResetModel(x => x.chromaticAberration, x => ref x.m_Settings);
            ResetModel(x => x.colorGrading, x => ref x.m_Settings);
            ResetModel(x => x.debugViews, x => ref x.m_Settings);
            ResetModel(x => x.depthOfField, x => ref x.m_Settings);
            ResetModel(x => x.dithering, x => ref x.m_Settings);
            ResetModel(x => x.eyeAdaptation, x => ref x.m_Settings);
            ResetModel(x => x.fog, x => ref x.m_Settings);
            ResetModel(x => x.grain, x => ref x.m_Settings);
            ResetModel(x => x.motionBlur, x => ref x.m_Settings);
            ResetModel(x => x.screenSpaceReflection, x => ref x.m_Settings);
            ResetModel(x => x.userLut, x => ref x.m_Settings);
            ResetModel(x => x.vignette, x => ref x.m_Settings);
            onUpdatePostProcessing?.Invoke(postProcessing);
        }

        internal delegate ref TSettings GetSettingsDelegate<TModel, TSettings>(TModel model);
        internal static void ResetModel<TModel, TSettings>(Func<PostProcessingProfile, TModel> getModel, GetSettingsDelegate<TModel, TSettings> getSettings)
            where TModel : PostProcessingModel
            where TSettings : struct
        {
            TModel originalModel = getModel(originalPostProcessing);
            TModel model = getModel(postProcessing);
            getSettings(model) = getSettings(originalModel);
            model.enabled = originalModel.enabled;
        }

        public static event Action<PostProcessingProfile> OnUpdatePostProcessing
        {
            add
            {
                onUpdatePostProcessing += value;
                Update(false);
            }
            remove
            {
                onUpdatePostProcessing -= value;
                Update(true);
            }
        }

        public static void RequestUpdate()
        {
            shouldUpdate = true;
        }
    }
}
