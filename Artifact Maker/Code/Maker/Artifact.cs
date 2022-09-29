using System;
using System.Collections.Generic;
using System.Linq;

namespace ArtifactMaker
{
    public class Artifact
    {
        //basic
        public string name = "";
        public string description = "";
        public SortedSet<string> scopes = new SortedSet<string>();
        public SortedDictionary<string, string> variables = new SortedDictionary<string, string>();//name , value
        public string type = "";
        public string visuals = "";
        public string rarity = "";
        public int? maxDurability = null;
        public bool decaying = true;
        public string? template = null;
        public int? wealth = null;//UI advanced
        public int? quality = null;//UI advanced
        public List<Tuple<string, int>> modifiers = new List<Tuple<string, int>>();//key:modifier value:level
        //advanced
        public List<Tuple<string, int>> improvements = new List<Tuple<string, int>>();


        private bool isValid()
        {
            if (String.IsNullOrEmpty(type) || String.IsNullOrEmpty(visuals) || String.IsNullOrEmpty(rarity))
            {
                return false;
            }
            if (maxDurability != null && maxDurability <= 0)
            {
                return false;
            }

            return true;
        }

        private List<string> basicGenerator()
        {
            List<string> info = new List<string>();

            info.Add("create_artifact = {");
            info.Add("    name = " + name);
            info.Add("    description = " + description);
            info.Add("    type = " + type);
            info.Add("    visuals = " + visuals);
            info.Add("    rarity = " + rarity);
            //***
            if (wealth != null)
            {
                info.Add("    wealth = " + wealth.ToString());
            }
            if (quality != null)
            {
                info.Add("    quality = " + quality.ToString());
            }
            //***
            if (maxDurability != null)
            {
                info.Add("    max_durability = " + maxDurability);
            }
            if (decaying == false)
            {
                info.Add("    decaying = no");
            }
            if (template != null)
            {
                info.Add("    template = " + template);
            }
            //modifiers
            foreach (var modifier in modifiers)
            {
                string temp = "    modifier = " + modifier.Item1;
                if (temp.IndexOf("_modifier$") < 0)
                {
                    temp += "_" + modifier.Item2.ToString() + "_modifier";
                }
                else
                {
                    temp = temp.Substring(0, temp.Length - 1);
                }

                info.Add(temp);
            }
            //scope
            if (isAdvanced())
            {
                info.Add("    save_scope_as = newly_created_artifact");
            }


            info.Add("}");

            return info;
        }

        private List<string> advancedGenerator()
        {
            var info = new List<string>
            {
                "scope:newly_created_artifact = {",
                "    save_scope_as = this_artifact"
            };
            foreach(var variable in variables)
            {
                info.Add("    set_variable = { name = " + variable.Key + " value = " + variable.Value + " }");
            }

            foreach (var improvement in improvements)
            {
                info.Add("");//empty line

                //improvements :
                string key = improvement.Item1;
                int level = improvement.Item2;
                switch (key)
                {
                    case "cursed_artifact"://notice
                        info.Add("    set_artifact_name = artifact.4000.cursed_name");
                        info.Add("    set_artifact_description = artifact.4000.cursed_desc");
                        info.Add("    set_variable = { name = cursed_artifact_var }");
                        switch (level)
                        {
                            case 1:
                                info.Add("    add_artifact_modifier = cursed_artifact_modifier");
                                break;
                            case 2:
                                info.Add("    add_artifact_modifier = very_cursed_artifact_modifier");
                                break;
                        }
                        break;

                    case "killer_weapon":
                        int valueKillerWeapon = 0;
                        if (level >= 1)
                        {
                            info.Add("    set_artifact_name = artifact_killer_1_name");
                            info.Add("    set_artifact_description = artifact_killer_1_desc");
                            info.Add("    add_artifact_modifier = artifact_killer_1_artifact_modifier");
                            valueKillerWeapon += 15;
                        }
                        if (level >= 2)
                        {
                            info.Add("    set_artifact_name = artifact_killer_2_name");
                            info.Add("    set_artifact_description = artifact_killer_2_desc");
                            info.Add("    add_artifact_modifier = artifact_killer_2_artifact_modifier");
                            valueKillerWeapon += 15;
                        }
                        if (level >= 3)
                        {
                            info.Add("    set_artifact_name = artifact_killer_3_name");
                            info.Add("    set_artifact_description = artifact_killer_3_desc");
                            info.Add("    add_artifact_modifier = artifact_killer_3_artifact_modifier");
                            valueKillerWeapon += 20;
                        }
                        if (level >= 4)
                        {
                            info.Add("    set_artifact_name = artifact_killer_4_name");
                            info.Add("    set_artifact_description = artifact_killer_4_desc");
                            info.Add("    add_artifact_modifier = artifact_killer_4_artifact_modifier");
                            valueKillerWeapon += 50;
                        }
                        info.Add("    set_variable = {");
                        info.Add("        name = artifact_killer_var");
                        info.Add("        value = " + valueKillerWeapon.ToString());
                        info.Add("    }");
                        break;

                    case "glorious_armor":
                        int valueGloriousArmor = 0;
                        if (level >= 1)
                        {
                            info.Add("    set_artifact_name = artifact_armor_1_name");
                            info.Add("    set_artifact_description = artifact_armor_1_desc");
                            info.Add("    add_artifact_modifier = artifact_armor_1_artifact_modifier");
                            valueGloriousArmor += 15;
                        }
                        if (level >= 2)
                        {
                            info.Add("    set_artifact_name = artifact_armor_2_name");
                            info.Add("    set_artifact_description = artifact_armor_2_desc");
                            info.Add("    add_artifact_modifier = artifact_armor_2_artifact_modifier");
                            valueGloriousArmor += 15;
                        }
                        if (level >= 3)
                        {
                            info.Add("    set_artifact_name = artifact_armor_3_name");
                            info.Add("    set_artifact_description = artifact_armor_3_desc");
                            info.Add("    add_artifact_modifier = artifact_armor_3_artifact_modifier");
                            valueGloriousArmor += 20;
                        }
                        if (level >= 4)
                        {
                            info.Add("    set_artifact_name = artifact_armor_4_name");
                            info.Add("    set_artifact_description = artifact_armor_4_desc");
                            info.Add("    add_artifact_modifier = artifact_armor_4_artifact_modifier");
                            valueGloriousArmor += 50;
                        }
                        info.Add("    set_variable = {");
                        info.Add("        name = artifact_armor_var");
                        info.Add("        value = " + valueGloriousArmor.ToString());
                        info.Add("    }");
                        break;

                    case "imperial_crown":
                        info.Add("    set_variable = artifact_named_crown_var");//scope?
                        info.Add("    set_artifact_name = artifact_emperors_crown_name");
                        info.Add("    set_artifact_description = artifact_emperors_crown_desc");
                        info.Add("    add_artifact_modifier = artifact_emperors_crown_artifact_modifier");
                        break;
                    case "crown_of_the_people":
                        info.Add("    set_variable = artifact_named_crown_var");
                        info.Add("    set_artifact_name = artifact_crown_special_name");
                        info.Add("    set_artifact_description = artifact_crown_special_desc");
                        info.Add("    add_artifact_modifier = artifact_crown_artifact_modifier");
                        break;

                    case "the_eye_of_the_expert_raid_speed_2_modifier":
                        info.Add("    add_artifact_modifier = artifact_raid_speed_2_modifier");
                        break;
                    case "the_eye_of_the_expert_heavy_cavalry_toughness_2_modifier":
                        info.Add("    add_artifact_modifier = artifact_heavy_cavalry_toughness_2_modifier");
                        break;
                    case "the_eye_of_the_expert_heavy_infantry_toughness_2_modifier":
                        info.Add("    add_artifact_modifier = artifact_heavy_infantry_toughness_2_modifier");
                        break;
                    case "the_eye_of_the_expert_knight_effectiveness_2_modifier":
                        info.Add("    add_artifact_modifier = artifact_knight_effectiveness_2_modifier");
                        break;
                    case "the_eye_of_the_expert_prowess_4_modifier":
                        info.Add("    add_artifact_modifier = artifact_prowess_4_modifier");
                        break;
                    case "the_eye_of_the_expert_heavy_infantry_toughness_3_modifier":
                        info.Add("    add_artifact_modifier = artifact_heavy_infantry_toughness_3_modifier");
                        break;

                    case "blessed_artifact":
                        info.Add("    set_variable = blessed_artifact_var");
                        info.Add("    add_artifact_modifier = blessed_artifact_modifier");
                        info.Add("    set_artifact_name = artifact_4060_blessed_artifact_name");
                        info.Add("    set_artifact_description = artifact_4060_blessed_artifact_desc");
                        break;

                    case "desecrated_relic":
                        info.Add("    add_artifact_modifier = desecrated_artifact_modifier");
                        info.Add("    set_variable = desecrated_artifact_var");
                        break;

                    case "dueling_weapon":
                        int valueDuelingWeapon = 0;
                        if (level >= 1)
                        {
                            info.Add("    set_artifact_name = artifact_duel_weapon_1_name");
                            info.Add("    set_artifact_description = artifact_duel_weapon_1_desc");
                            info.Add("    add_artifact_modifier = artifact_duel_weapon_1_artifact_modifier");
                            valueDuelingWeapon += 5;
                        }
                        if (level >= 2)
                        {
                            info.Add("    set_artifact_name = artifact_duel_weapon_2_name");
                            info.Add("    set_artifact_description = artifact_duel_weapon_2_desc");
                            info.Add("    add_artifact_modifier = artifact_duel_weapon_2_artifact_modifier");
                            valueDuelingWeapon += 5;
                        }
                        if (level >= 3)
                        {
                            info.Add("    set_artifact_name = artifact_duel_weapon_3_name");
                            info.Add("    set_artifact_description = artifact_duel_weapon_3_desc");
                            info.Add("    add_artifact_modifier = artifact_duel_weapon_3_artifact_modifier");
                            valueDuelingWeapon += 10;
                        }
                        if (level >= 4)
                        {
                            info.Add("    set_artifact_name = artifact_duel_weapon_4_name");
                            info.Add("    set_artifact_description = artifact_duel_weapon_4_desc");
                            info.Add("    add_artifact_modifier = artifact_duel_weapon_4_artifact_modifier");
                            valueDuelingWeapon += 10;
                        }
                        info.Add("    set_variable = {");
                        info.Add("        name = artifact_duel_weapon_var");
                        info.Add("        value = " + valueDuelingWeapon.ToString());
                        info.Add("    }");
                        break;

                    default:
                        break;
                }
            }

            info.Add("}");

            return info;
        }

        public string generator()
        {
            if (!isValid())
            {
                return "";
            }

            string info = "";

            string indentation;
            if (isAdvanced())
            {
                indentation = "    ";
            }
            else
            {
                indentation = "";
            }

            //start
            if (isAdvanced())
            {
                info += "create_artifact_weapon_effect = {" + "\r\n" + "    $OWNER$ = { save_scope_as = owner }" + "\r\n\r\n";

                if (wealth != null)
                {
                    info += "    save_scope_value_as = { name = wealth value = " + wealth + " }\r\n";
                }
                if (quality != null)
                {
                    info += "    save_scope_value_as = { name = quality value = " + quality + " }\r\n";
                }
                if (wealth != null || quality != null)
                {
                    info += "\r\n";
                }

                foreach(string scope in scopes)
                {
                    info += "    " + scope + "\r\n";
                }
                if (scopes.Count > 0)
                {
                    info += "\r\n";
                }
            }

            //basic
            List<string> basic = basicGenerator();
            foreach (var line in basic)
            {
                info += indentation + line + "\r\n";
            }

            if (improvements.Find(a => a.Item1 == "crown_of_the_people") != null)
            {
                info += "\r\n" + "    primary_title = { save_scope_as = primary_title_scope }" + "\r\n";
            }

            //advanced
            if (isAdvanced())
            {
                List<string> advanced = advancedGenerator();
                info += "\r\n";
                foreach (var line in advanced)
                {
                    info += indentation + line + "\r\n";
                }
            }

            //end
            if (isAdvanced())
            {
                info += "}" + "\r\n";
            }

            return info;
        }

        private bool isAdvanced()
        {
            if (improvements.Count > 0 || scopes.Count > 0 || wealth != null || quality != null || variables.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
