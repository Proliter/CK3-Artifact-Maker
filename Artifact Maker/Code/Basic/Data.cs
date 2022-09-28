using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ArtifactMaker
{
    public class Data
    {
        //singleton
        private static Data instance = new Data();

        protected Dictionary<string, List<string>> data;
        public static int version = 0;

        public static Data getInstance()
        {
            return instance;
        }

        private Data()
        {
            string path = "Artifact_Maker.Info.Data.txt";

            //get file
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream? stream = assembly.GetManifestResourceStream(path);
            if (stream == null)
            {
                throw new Exception("Find not Data.");
            }

            using (StreamReader file = new StreamReader(stream))
            {
                string? line;
                data = new Dictionary<string, List<string>>();

                string? header = null;
                List<string>? info = null;
                //read
                while ((line = file.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Count() > 0)
                    {
                        switch (line[0])
                        {
                            case '#':
                                if (header == null || info == null)
                                {
                                    header = line.Substring(1).Trim();
                                    info = new List<string>();
                                }
                                else
                                {
                                    if (!data.TryAdd(header, info))
                                    {
                                        throw new Exception("Data file corrupted.");
                                    }

                                    header = line.Substring(1).Trim();
                                    info = new List<string>();
                                }
                                break;
                            case '=':
                                if (header != null && info != null)
                                {
                                    info.Add(line.Substring(1).Trim());
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                //end
                if (header != null && info != null)
                {
                    data.Add(header, info);
                }
            }
        }

        public static List<string> get(string key)
        {
            List<string>? value;
            if (!instance.data.TryGetValue(key, out value))
            {
                throw new Exception("Data corrupted");
            }

            return value;
        }

        public static void addInfo(string key, string info)
        {
            if (instance.data.ContainsKey(key))
            {
                List<string> temp = instance.data[key];
                temp.Add(info);
            }
            else
            {
                List<string> temp = new List<string>();
                temp.Add(info);
                instance.data.Add(key, temp);
            }
        }

        public static void addKey(string key, List<string> info)
        {
            if (instance.data.ContainsKey(key))
            {
                List<string> temp = instance.data[key];
                foreach (string item in info)
                {
                    temp.Add(item);
                }
            }
            else
            {
                instance.data.Add(key, info);
            }
        }

        public static void loadDataSecondLevel(List<string> key, List<List<string>> target, string prefix = "")
        {
            for (int i = 0; i < key.Count; i++)
            {
                List<string> temp = Data.get(prefix + key[i]);
                target.Add(temp);
            }
        }
    }
}
