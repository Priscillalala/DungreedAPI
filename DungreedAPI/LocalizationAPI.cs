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
    public static class LocalizationAPI
    {
        internal static ConfigEntry<string> defaultLanguage;
        internal static Dictionary<string, List<KeyValuePair<string, string>>> managedDataByLanguage = new Dictionary<string, List<KeyValuePair<string, string>>>();
        internal static List<string> xmlPaths = new List<string>();
        internal static bool alreadyAdded = false;

        internal static void Init()
        {
            defaultLanguage = DungreedAPI.config.Bind("Localization", "Default Language", "english", "A fallback language for tokens if they do not have a value in the current language.");
            On.MyLocalization.ctor += MyLocalization_ctor;
            On.MyLocalization.GetTextValue += MyLocalization_GetTextValue;
        }

        private static void MyLocalization_ctor(On.MyLocalization.orig_ctor orig, MyLocalization self)
        {
            orig(self);
            foreach (string path in xmlPaths)
            {
                try
                {
                    LoadXmlFile(path, self);
                }
                catch (Exception ex)
                {
                    DungreedAPI.logger.LogError($"Error while loading xml file at {path}: {ex}");
                }
            }
            xmlPaths = null;
            foreach (KeyValuePair<string, List<KeyValuePair<string, string>>> data in managedDataByLanguage)
            {
                if (self.textValues.TryGetValue(data.Key, out Dictionary<string, string> lang))
                {
                    foreach (KeyValuePair<string, string> pair in data.Value)
                    {
                        lang[pair.Key] = pair.Value;
                    }
                }
            }
            managedDataByLanguage = null;
            alreadyAdded = true;
        }

        internal static void LoadXmlFile(string path, MyLocalization localization)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            XmlNodeList childNodes = xmlDocument.DocumentElement.ChildNodes;
            for (int i = 0; i < childNodes.Count; i++)
            {
                string key = childNodes[i].Attributes["name"].Value;
                XmlNodeList childNodes2 = childNodes[i].ChildNodes;
                for (int j = 0; j < childNodes2.Count; j++)
                {
                    if (localization.textValues.TryGetValue(childNodes2[j].Name, out Dictionary<string, string> lang))
                    {
                        lang[key] = childNodes2[j].InnerText;
                    }
                }
            }
        }

        private static string MyLocalization_GetTextValue(On.MyLocalization.orig_GetTextValue orig, MyLocalization self, string key)
        {
            string result = orig(self, key);
            if (result.Equals(string.Empty) && key != null && (!self.textValues.TryGetValue(self.currentLanguage, out Dictionary<string, string> currentLang) || !currentLang.ContainsKey(key.Trim())))
            {
                if (self.textValues.TryGetValue(defaultLanguage.Value, out Dictionary<string, string> defaultLang))
                {
                    defaultLang.TryGetValue(key.Trim(), out result);
                } 
            }
            return result;
        }

        public static void Add(string key, string value, Optional<string> language = default)
        {
            string langKey = language.Exists ? language.Value : "english";
            if (alreadyAdded)
            {
                if (MyLocalization.Instance.textValues.TryGetValue(langKey, out Dictionary<string, string> lang))
                {
                    lang[key] = value;
                }
                return;
            }
            if (!managedDataByLanguage.TryGetValue(langKey, out List<KeyValuePair<string, string>> list))
            {
                list = new List<KeyValuePair<string, string>>();
                managedDataByLanguage[langKey] = list;
            }
            list.Add(new KeyValuePair<string, string>(key, value));
        }

        public static void AddMany(params (string key, string value)[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Add(data[i].key, data[i].value);
            }
        }

        public static void AddMany(string language, params (string key, string value)[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Add(data[i].key, data[i].value, language);
            }
        }

        public static void AddXmlFile(string path)
        {
            if (alreadyAdded)
            {
                LoadXmlFile(path, MyLocalization.Instance);
                return;
            }
            xmlPaths.Add(path);
        }
    }
}
