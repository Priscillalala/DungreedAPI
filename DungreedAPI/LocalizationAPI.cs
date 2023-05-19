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
#pragma warning disable
    /// <summary>
    /// Add localization directly or load localization files.
    /// </summary>
    /// <remarks>
    /// <list>
    /// <item><term><see cref="Add(string, string, string)"/></term><description>Add a single localization key and value pair.</description></item>
    /// <item><term><see cref="AddMany(string, (string key, string value)[])"/></term><description>Add many localization keys and values to a single language.</description></item>
    /// <item><term><see cref="AddMany((string key, string value)[])"/></term><description>Add many localization keys and values to <see cref="Languages.English"/>.</description></item>
    /// <item><term><see cref="AddManyForKey(string, (string language, string value)[])"/></term><description>Add many localization values to a single localization key.</description></item>
    /// <item><term><see cref="AddXmlFile(string)"/></term><description>dd an XML localization file in the same format as the Dungreed localization file.</description></item>
    /// </list>
    /// </remarks>
#pragma warning restore
    public static class LocalizationAPI
    {
        internal static ConfigEntry<string> fallbackLanguage;
        internal static Dictionary<string, List<KeyValuePair<string, string>>> managedDataByLanguage = new Dictionary<string, List<KeyValuePair<string, string>>>();
        internal static List<string> xmlPaths = new List<string>();
        internal static bool alreadyAdded = false;

        internal static void Init()
        {
            fallbackLanguage = DungreedAPI.config.Bind("Localization", "Fallback Language", Languages.English, "A fallback language for tokens if they do not have a value in the current language.");
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
                if (self.textValues.TryGetValue(fallbackLanguage.Value, out Dictionary<string, string> defaultLang))
                {
                    defaultLang.TryGetValue(key.Trim(), out result);
                }
                else
                {
                    result = key;
                }
            }
            return result;
        }

        /// <summary>
        /// Add a single localization key and value pair.
        /// </summary>
        /// <param name="key">A localization key.</param>
        /// <param name="value">A localization value.</param>
        /// <param name="language">A language identifier.</param>
        public static void Add(string key, string value, string language = Languages.English)
        {
            if (key == null || value == null)
            {
                return;
            }
            if (alreadyAdded)
            {
                if (MyLocalization.Instance.textValues.TryGetValue(language, out Dictionary<string, string> lang))
                {
                    lang[key] = value;
                }
                return;
            }
            if (!managedDataByLanguage.TryGetValue(language, out List<KeyValuePair<string, string>> list))
            {
                list = new List<KeyValuePair<string, string>>();
                managedDataByLanguage[language] = list;
            }
            list.Add(new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// Add many localization keys and values to a single language.
        /// </summary>
        /// <param name="language">A language identifier.</param>
        /// <param name="data">Pairs of localization keys and values.</param>
        public static void AddMany(string language, params (string key, string value)[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Add(data[i].key, data[i].value, language);
            }
        }

        /// <summary>
        /// Add many localization keys and values to <see cref="Languages.English"/>.
        /// </summary>
        /// <param name="data">Pairs of localization keys and values.</param>
        public static void AddMany(params (string key, string value)[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Add(data[i].key, data[i].value);
            }
        }

        /// <summary>
        /// Add many localization values to a single localization key.
        /// </summary>
        /// <param name="key">A localization key.</param>
        /// <param name="data">Pairs of language identifiers and localization values.</param>
        public static void AddManyForKey(string key, params (string language, string value)[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Add(key, data[i].value, data[i].language);
            }
        }

        /// <summary>
        /// Add an XML localization file in the same format as the Dungreed localization file.
        /// </summary>
        /// <param name="path">An absolute path to the localization file.</param>
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
