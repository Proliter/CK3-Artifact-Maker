using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace ArtifactMaker
{
    public class Localisation
    {
        private string language;
        protected Dictionary<string, string> data;

        private static Localisation instance = new Localisation();
        protected static Localisation? defaultLanguageLocalisation = null;//show with this if missing

        private Localisation(string arg = "")
        {
            string appLanguage;

            //use app language if have not arg
            if (String.IsNullOrEmpty(arg))
            {
                appLanguage = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            }
            else
            {
                appLanguage = arg;
            }

            //use default if the language has not localisation
            localisationReadFile(appLanguage, new HashSet<string>());
            if (language == null)
            {
                localisationReadFile(Config.get("default_language"), new HashSet<string>());
            }

            if (language == null || data == null)
            {
                throw new Exception("Error: Localisation File Error");
            }

            //construct default_language
            if (defaultLanguageLocalisation == null)
            {
                if (Config.get("default_language") == language)
                {
                    defaultLanguageLocalisation = this;
                }
                else
                {
                    defaultLanguageLocalisation = new Localisation(Config.get("default_language"));
                }
            }
        }

        public static Localisation getInstance()
        {
            return instance;
        }

        public static void changeLanguage(string arg = "")
        {
            instance = new Localisation(arg);
        }

        private void localisationReadFile(string language, HashSet<string> historyArg)
        {
            //avoid infinite loop
            if (historyArg.Contains(language))
            {
                return;
            }
            else
            {
                historyArg.Add(language);
            }

            string path = "Artifact_Maker.Localisation." + language + ".txt";

            //get file
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream? stream = assembly.GetManifestResourceStream(path);
            if (stream == null)
            {
                if (language.Contains('-'))
                {
                    localisationReadFile(language.Substring(0, 2), historyArg);
                    return;
                }
                else
                {
                    return;
                }
            }

            //open file
            StreamReader file = new StreamReader(stream);

            //check language
            string? targetLanguage = file.ReadLine();
            if (targetLanguage == null)
            {
                return;
            }
            targetLanguage = targetLanguage.Substring(targetLanguage.IndexOf('#') + 1).Trim();
            if (targetLanguage != language)
            {
                file.Close();
                localisationReadFile(targetLanguage, historyArg);
                return;
            }

            //read file
            string? line;
            data = new Dictionary<string, string>();
            while ((line = file.ReadLine()) != null)
            {
                if (line.Count() == 0)
                {
                    continue;
                }

                line = line.Replace(@"\r\n", Environment.NewLine);

                int flag = line.IndexOf('=', 1, line.Length - 2);
                if (flag != -1)
                {
                    string key = line.Substring(0, flag).Trim();
                    string value = line.Substring(flag + 1);

                    if (!data.TryAdd(key, value))
                    {
                        throw new Exception("Localisation file corrupted.");
                    }
                }
            }
            this.language = targetLanguage;

            file.Close();

            return;
        }

        public static string get(string key)
        {
            string? temp;

            if (!instance.data.TryGetValue(key, out temp))
            {
                if (defaultLanguageLocalisation != null)
                {
                    defaultLanguageLocalisation.data.TryGetValue(key, out temp);
                }
            }

            if (temp == null)
            {
                temp = key;
            }

            return temp;
        }

        public static void add(string key, string value)
        {
            instance.data.TryAdd(key, value);
        }

        public static void applyLocalisation(Grid sender)
        {
            foreach (var item in sender.Children)
            {
                string itemType = item.GetType().Name;
                if (itemType == "Grid")
                {
                    applyLocalisation((Grid)item);
                    continue;
                }

                //check
                switch (itemType)
                {
                    case "Label":
                    case "CheckBox":
                    case "Button":
                        break;
                    default:
                        continue;
                }
                
                string objectName = ((Control)item).Name;
                string transferedName;
                switch (itemType)
                {
                    case "Label":
                        transferedName = objectName.Replace("label", "Label_");
                        break;
                    case "CheckBox":
                        transferedName = objectName.Replace("checkbox", "CheckBox_");
                        break;
                    case "Button":
                        transferedName = objectName.Replace("button", "Button_");
                        break;
                    default:
                        continue;
                }

                string result = get(transferedName);
                switch (itemType)
                {
                    case "Label":
                        ((Label)item).Content = result;
                        break;
                    case "CheckBox":
                        ((CheckBox)item).Content = result;
                        break;
                    case "Button":
                        ((Button)item).Content = result;
                        break;
                    default:
                        break;
                }
            }
        }

        public static string getLanguage()
        {
            return instance.language;
        }

        public static void loadLocalisationFirstLevel(List<string> key, List<string> target, string prefix = "")
        {
            target.Clear();

            foreach (var item in key)
            {
                string trans = prefix + item;
                if (trans.Contains('@'))
                {
                    trans = trans.Substring(0, trans.IndexOf('@'));//delete chars after @
                }

                string temp = get(trans);
                target.Add(temp);
            }
        }

        public static void loadLocalisationSecondLevel(List<List<string>> key, List<List<string>> target, string prefix = "")
        {
            target.Clear();

            for (int i = 0; i < key.Count; i++)
            {
                List<string> temp = new List<string>();
                loadLocalisationFirstLevel(key[i], temp, prefix);
                target.Add(temp);
            }
        }
    }
}
