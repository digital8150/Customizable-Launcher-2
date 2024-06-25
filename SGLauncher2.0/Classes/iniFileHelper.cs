using HandyControl.Controls;
using IniParser;
using IniParser.Model;
using System;

namespace SGLauncher2._0.Classes
{
    internal class iniFileHelper
    {
        private string FileName;
        private FileIniDataParser fileIniDataParser;
        private IniData parsedData;

        public iniFileHelper(string filename)
        {
            try
            {
                FileName = filename;
                fileIniDataParser = new FileIniDataParser();
                parsedData = fileIniDataParser.ReadFile(FileName);
            }
            catch (Exception ex)
            {
                Growl.Warning($"{ex}");
            }

        }

        public iniFileHelper()
        {

        }

        private void Save()
        {
            try
            {
                fileIniDataParser.WriteFile(FileName, parsedData);
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex}");
            }


        }

        public void changeValue(string header, string keyname, string value)
        {
            try
            {
                parsedData[header].GetKeyData(keyname).Value = value;
                Save();
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex}");
            }

        }

        public string getValue(string header, string keyname)
        {
            string result = "";
            try
            {
                result = parsedData[header].GetKeyData(keyname).Value;
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex}");

            }
            return result;

        }

        public bool getValue_bool(string header, string keyname)
        {

            bool result = false;
            try
            {
                if (parsedData[header].GetKeyData(keyname).Value == "true" || parsedData[header].GetKeyData(keyname).Value == "True" || parsedData[header].GetKeyData(keyname).Value == "1")
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex}");
            }

            return result;

        }
    }
}
