using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ArtifactMaker
{
    public class Config
    {
        //singleton
        private static Config instance = new Config();

        protected Dictionary<string, string> config;

        public static Config getInstance()
        {
            return instance;
        }

        private Config()
        {
            string path = "Artifact_Maker.Info.Config.txt";

            //get file
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream? stream = assembly.GetManifestResourceStream(path);
            if (stream == null)
            {
                throw new Exception("Find not config.");
            }

            using (StreamReader file = new StreamReader(stream))
            {
                string? line;
                config = new Dictionary<string, string>();
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length <= 0)
                    {
                        continue;
                    }

                    int flag = line.IndexOf('=', 1, line.Length - 2);
                    if (flag != -1)
                    {
                        string key = line.Substring(0, flag).Trim();
                        string value = line.Substring(flag + 1);

                        if (!config.TryAdd(key, value))
                        {
                            throw new Exception("Config file corrupted.");
                        }
                    }
                }
            }
        }

        public static string get(string key)
        {
            string? value;
            if (!instance.config.TryGetValue(key, out value))
            {
                throw new Exception("Config corrupted");
            }

            return value;
        }

        public static void change(string key, string value)
        {
            if (instance.config.ContainsKey(key))
            {
                instance.config[key] = value;
            }
        }
    }
}
