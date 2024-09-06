using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ShadowSXLauncher
{
    public class DolphinSettingsAdjuster
    {
        private string filePath;
        public Dictionary<string, List<string>> Sections;

        private string dataSingleString
        {
            get
            {
                var singleString = string.Empty;

                foreach (var key in Sections.Keys)
                {
                    singleString += "[" + key + "]" + Environment.NewLine;
                    Sections[key].ForEach(line => singleString += line + Environment.NewLine);
                }

                return singleString.TrimEnd(Environment.NewLine.ToCharArray());
            }
        }
        
        public DolphinSettingsAdjuster(string filePath)
        {
            this.filePath = filePath;
            Sections = new Dictionary<string, List<string>>();
            
            if (File.Exists(filePath))
            {
                var fileContents = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(fileContents))
                {
                    var matchString = @"^\[\w*\]";
                    var matches = Regex.Matches(fileContents, matchString, RegexOptions.Multiline);
                    var sectionHeaders = new List<string>();

                    for (int i = 0; i < matches.Count; i++)
                    {
                        sectionHeaders.Add(matches[i].Value);
                    }

                    var fileContentsMultiLine =
                        fileContents.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    var currentKey = "";
                    foreach (var line in fileContentsMultiLine)
                    {
                        if (sectionHeaders.Contains(line))
                        {
                            var key = line.Remove(line.Length - 1).Remove(0, 1);
                            Sections.Add(key, new List<string>());
                            currentKey = key;
                        }
                        else if (!string.IsNullOrEmpty(currentKey))
                        {
                            Sections[currentKey].Add(line);
                        }
                        else
                        {
                            throw new Exception("Attempted to add a line without preparing the Dictionary Key.");
                        }
                    }
                }
            }
        }

        public void SaveSettings()
        {
            File.WriteAllText(filePath, dataSingleString);
        }
        
        public void SetSetting(string section, string setting, double value)
        {
            SetSetting(section, setting, value.ToString());
        }

        public void SetSetting(string section, string setting, bool value)
        {
            SetSetting(section, setting, value.ToString());
        }
        
        public void SetSetting(string section, string setting, string value)
        {
            if (!Sections.ContainsKey(section))
            {
                Sections.Add(section, new List<string>());
            }
            var foundSettingIndex = Sections[section].FindIndex(s => s.Contains(setting));
            if (foundSettingIndex != -1)
            {
                Sections[section][foundSettingIndex] = setting + " = " + value;
            }
            else
            {
                Sections[section].Add(setting + " = " + value);
            }
        }
        
        public void RemoveSetting(string section, string setting)
        {
            //Section needs to exist to properly remove setting.
            if (Sections.ContainsKey(section))
            {
                //Find setting.
                var foundSetting = Sections[section].FindIndex(s => s.Contains(setting));
                if (foundSetting != -1)
                {
                    Sections[section].RemoveAt(foundSetting);
                }
            }
        }

        public void InstallGeckoCode(string name, string authors, string code, string description)
        {
            var section = "Gecko";
            if (!Sections.ContainsKey(section))
            {
                Sections.Add(section, new List<string>());
            }

            var codeSearchName = "$" + name + " [" + authors + "]";
            var foundCode = Sections[section].Find(s => s.Contains(name));
            if (foundCode != null)
            {
                UninstallGeckoCode(name);
            }
            
            Sections[section].Add(codeSearchName);
            Sections[section].Add(code);
            //This should update the description to include the * for each line of description
            var processedDescription = "*" + description.Replace("\n", "\n*");
            Sections[section].Add(processedDescription);
        }
        
        public void UninstallGeckoCode(string name)
        {
            var section = "Gecko";
            if (!Sections.ContainsKey(section))
            {
                Sections.Add(section, new List<string>());
            }
            
            var foundIndex = Sections[section].FindIndex(s => s.Contains(name));
            if (foundIndex != -1)
            {
                EnableGeckoCode(name, false);
                
                //Delete all lines from found index down until $ is hit or end of list.
                //$ indicates a new code.
                var endFound = false;
                var linesToRemove = 1;
                for (int i = foundIndex + 1; i < Sections[section].Count && !endFound; i++)
                {
                    //Check first character for $, which will only appear for a new code.
                    if (Sections[section][i][0] == '$')
                    {
                        linesToRemove++;
                    }
                }
                Sections[section].RemoveRange(foundIndex, linesToRemove);
            }
        }

        public void EnableGeckoCode(string name, bool value)
        {
            var section = "Gecko_Enabled";
            var item = "$" + name;
            if (!Sections.ContainsKey(section))
            {
                Sections.Add(section, new List<string>());
            }
            var foundSetting = Sections[section].Find(s => s.Contains(item));
            if (foundSetting != null && !value)
            {
                Sections[section].Remove(item);
            }
            else if(foundSetting == null && value)
            {
                Sections[section].Add(item);
            }
        }
    }
}