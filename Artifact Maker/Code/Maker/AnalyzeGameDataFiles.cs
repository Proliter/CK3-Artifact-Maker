using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ArtifactMaker
{
    internal static class AnalyzeGameDataFiles
    {
        static public void applyAdaption()
        {
            Data.version++;

            applyType();
            applyVisual();
            applyTemplate();

            applyModifier();
        }

        //apply
        static void applyType()
        {
            string path = MainWindow.gamePath + @"\game\common\artifacts\types\00_types.txt";

            List<Block> typeBlock = analyze(preproccess(path));

            List<string> oldType = new List<string>();
            foreach (string str in Data.get("type_category"))
            {
                foreach (string str2 in Data.get("type_detail_" + str))
                {
                    oldType.Add(str2);
                }
            }

            typeBlock = removeDuplicates(typeBlock, oldType);

            List<string> newType = new List<string>();
            foreach (Block block in typeBlock)
            {
                newType.Add(block.header);
            }

            if (newType.Count > 0)
            {
                Data.addInfo("type_category", "special_unclassified");
                Data.addKey("type_detail_special_unclassified", newType);
            }

            if (Localisation.get("type_special_unclassified") == "type_special_unclassified")
            {
                Localisation.add("type_special_unclassified", Localisation.get("special_unclassified"));
            }
        }

        static void applyVisual()
        {
            string path = MainWindow.gamePath + @"\game\common\artifacts\visuals\";

            var visualTxt = Directory.EnumerateFiles(path, "*.txt");
            foreach (string pathTxt in visualTxt)
            {
                string fileName = pathTxt.Substring(path.Length);

                string category;
                try
                {
                    category = Data.get("file_" + fileName.Remove(fileName.Length - 4))[0];
                }
                catch
                {
                    category = fileName.Remove(fileName.Length - 4);//remove ".txt"
                    Data.addInfo("file_" + category, category);
                    Data.addInfo("visual_category", category);
                }

                List<Block> visualBlock = analyze(preproccess(pathTxt));
                try
                {
                    visualBlock = removeDuplicates(visualBlock, Data.get("visual_detail_" + category));
                }
                catch
                {
                }

                List<string> newVisual = new List<string>();
                foreach (Block block in visualBlock)
                {
                    newVisual.Add(block.header);
                }


                Data.addKey("visual_detail_" + category, newVisual);

                foreach (Block block in visualBlock)
                {
                    List<string> icons = new List<string>();

                    Regex regex = new Regex(@"([^ |"" |=]+\.dds)");
                    foreach (string line in block.body)
                    {
                        var temp = regex.Matches(line);
                        foreach (Match match in temp)
                        {
                            icons.Add(match.Groups[1].Value);
                        }
                    }

                    Data.addKey("icon_" + block.header, icons);
                }
            }
        }

        static void applyTemplate()
        {
            string path = MainWindow.gamePath + @"\game\common\artifacts\templates\";

            var templateTxt = Directory.EnumerateFiles(path, "*.txt");
            foreach (string pathTxt in templateTxt)
            {
                string fileName = pathTxt.Substring(path.Length);

                string category;
                try
                {
                    category = Data.get("file_" + fileName.Remove(fileName.Length - 4))[0];
                }
                catch
                {
                    category = fileName.Remove(fileName.Length - 4);//remove ".txt"
                    Data.addInfo("file_" + category, category);
                    Data.addInfo("template_category", category);
                }

                List<Block> templateBlock = analyze(preproccess(pathTxt));
                try
                {
                    templateBlock = removeDuplicates(templateBlock, Data.get(category));
                }
                catch
                {
                }

                List<string> newVisual = new List<string>();
                foreach (Block block in templateBlock)
                {
                    newVisual.Add(block.header);
                }

                Data.addKey(category, newVisual);
            }
        }

        static void applyModifier()//shit
        {
            string path = MainWindow.gamePath + @"\game\common\modifiers\00_artifact_modifiers.txt";

            List<Block> modifierBlock = analyze(preproccess(path));

            Dictionary<string, List<Block>> sortableModifiers = new Dictionary<string, List<Block>>();
            List<Block> unsortableModifiers = new List<Block>();

            //classify
            foreach (Block block in modifierBlock)
            {
                //for safety
                Regex regexArtifact = new Regex(@"artifact_.+_modifier");
                if (!regexArtifact.IsMatch(block.header))
                {
                    continue;
                }

                Regex regexIsNum = new Regex(@"(artifact_.+)_\d+_modifier");
                if (regexIsNum.IsMatch(block.header))
                {
                    string key = regexIsNum.Matches(block.header)[0].Groups[1].Value;

                    List<Block>? temp;
                    if (!sortableModifiers.TryGetValue(key, out temp))
                    {
                        temp = new List<Block>();
                        sortableModifiers.Add(key, temp);
                    }
                    temp.Add(block);
                }
                else
                {
                    unsortableModifiers.Add(block);
                }
            }

            //sortable: sort
            foreach (var blocksPair in sortableModifiers)
            {
                var target = blocksPair.Value;

                List<Tuple<int, Block>> temp = new List<Tuple<int, Block>>();

                Regex regex = new Regex(@"artifact_.+_(\d+)_modifier");
                foreach (Block block in target)
                {
                    var index = Int32.Parse(regex.Matches(block.header)[0].Groups[1].Value);
                    temp.Add(new Tuple<int, Block>(index, block));
                }

                temp.Sort((a, b) =>
                {
                    return a.Item1 - b.Item1;
                });

                target = new List<Block>();
                foreach (var block in temp)
                {
                    target.Add(block.Item2);
                }
            }

            //sortable
            foreach (var blocks in sortableModifiers)
            {
                List<string> modifierValues = new List<string>();

                //add if category not contained
                try
                {
                    Data.get(@"value_" + blocks.Key);
                }
                catch
                {
                    if (!Data.get("modifier_category").Contains("special_unclassified"))
                    {
                        Data.addInfo("modifier_category", "special_unclassified");
                    }

                    Data.addInfo("modifier_detail_special_unclassified", blocks.Key);
                    //Localisation
                    string trans = blocks.Key;
                    Localisation.add("modifier_" + blocks.Key, trans);
                }

                //rewrite modifiers data value
                foreach (var block in blocks.Value)
                {
                    string value = "";

                    //get value
                    Regex regex = new Regex(@"-?(\d+\.)?\d+");
                    foreach (string line in block.body)
                    {
                        var temp = regex.Matches(line);
                        foreach (Match match in temp)
                        {
                            value += match.Groups[0].Value + " ";
                        }
                    }
                    if (value.Length > 0)
                    {
                        value = value.Substring(0, value.Length - 1);
                    }

                    modifierValues.Add(value);
                }

                //rewrite
                try
                {
                    var temp = Data.get(@"value_" + blocks.Key);

                    bool tempFlag = true;
                    foreach (string str in temp)
                    {
                        if (str.Contains("Invalid"))
                        {
                            tempFlag = false;
                            break;
                        }
                    }

                    if (tempFlag)
                    {
                        temp.Clear();
                        temp.AddRange(modifierValues);
                    }
                }
                catch
                {
                    Data.addKey(@"value_" + blocks.Key, modifierValues);
                }
            }

            //unsortable
            foreach (Block block in unsortableModifiers)
            {
                //add if category not contained
                try
                {
                    Data.get(@"value_" + block.header);
                }
                catch
                {
                    if (!Data.get("modifier_category").Contains("special_unclassified"))
                    {
                        Data.addInfo("modifier_category", "special_unclassified");
                    }

                    Data.addInfo("modifier_detail_special_unclassified", block.header);
                    //Localisation
                    string trans = block.header;
                    Localisation.add("modifier_" + block.header, trans);
                }

                //rewrite modifiers data value
                string value = "";
                Regex regex = new Regex(@"-?(\d+\.)?\d+");
                foreach (string line in block.body)
                {
                    var temp = regex.Matches(line);
                    foreach (Match match in temp)
                    {
                        value += match.Groups[0].Value + " ";
                    }
                }
                if (value.Length > 0)
                {
                    value = value.Substring(0, value.Length - 1);
                }

                //rewrite
                try
                {
                    var temp = Data.get(@"value_" + block.header);
                    temp.Clear();
                    temp.Add(value);
                }
                catch
                {
                    Data.addInfo(@"value_" + block.header, value);
                }
            }

            if (Localisation.get("modifier_special_unclassified") == "modifier_special_unclassified")
            {
                Localisation.add("modifier_special_unclassified", Localisation.get("special_unclassified"));
            }
        }

        //api
        struct Block
        {
            public string header;
            public List<string> body;
        }

        static List<Block> analyze(List<string> strings)
        {
            List<Block> res = new List<Block>();

            //multi to single
            StringBuilder source = new StringBuilder("", strings.Count * 50);
            foreach (string line in strings)
            {
                source.Append(line).Append('\n');
            }

            //analyze
            Block block = new Block();
            block.header = "";
            block.body = new List<string>();

            string header = "";
            int length = source.Length;
            int i = 0;
            while (i < length)
            {
                switch (source[i])
                {
                    case ' ':
                    case '\n':
                    case '=':
                        if (!String.IsNullOrEmpty(header))
                        {
                            block.header = header;
                            header = "";
                        }
                        break;
                    case '{':
                        //body
                        StringBuilder bodyTemp = new StringBuilder("");

                        int braceNum = 0;
                        do
                        {
                            switch (source[i])
                            {
                                case '{':
                                    ++braceNum;
                                    break;
                                case '}':
                                    --braceNum;
                                    break;
                                default:
                                    break;
                            }

                            bodyTemp.Append(source[i]);
                            ++i;
                        } while (braceNum > 0);


                        string bodyInfo = bodyTemp.ToString();
                        bodyInfo = bodyInfo.Substring(1, bodyTemp.Length - 2) + '\n';
                        Regex regex = new Regex(@"([^\n]+)\n");
                        var regexTemp = regex.Matches(bodyInfo);
                        foreach (Match match in regexTemp)
                        {
                            block.body.Add(match.Groups[1].Value);
                        }

                        res.Add(block);

                        block = new Block();
                        block.header = "";
                        block.body = new List<string>();

                        break;
                    default:
                        header += source[i];
                        break;
                }

                ++i;
            }

            return res;
        }

        static List<string> preproccess(string path)
        {
            List<string> res = new List<string>();
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string? line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        //remove annotation
                        int temp = line.IndexOf('#');
                        if (temp >= 0)
                        {
                            line = line.Substring(0, temp);
                        }

                        res.Add(line);
                    }
                }
            }
            catch
            {
                return new List<string>();
            }

            return res;
        }

        static List<Block> removeDuplicates(List<Block> source, List<string> old)
        {
            var res = new List<Block>();

            foreach (Block block in source)
            {
                if (!old.Contains(block.header))
                {
                    res.Add(block);
                }
            }

            return res;
        }
    }
}
