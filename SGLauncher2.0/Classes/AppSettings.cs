using HandyControl.Controls;
using IniParser;
using IniParser.Model;
using System;

namespace SGLauncher2._0.Classes
{
    internal class AppSettings
    {
        private static string FileName = "app.conf";

        private static FileIniDataParser fileIniDataParser = new FileIniDataParser();
        private static IniData parsedData = fileIniDataParser.ReadFile(FileName);

        //런처 설정 데이터
        private static bool b_isFirstStartup = getValue_bool("Settings", "b_isFirstStartup", "True");
        private static bool b_useOnline = getValue_bool("Settings", "b_useOnline", "True");
        private static bool b_useExperimental = getValue_bool("Settings", "b_useExperimental", "True");
        private static bool b_useShortcut = getValue_bool("Settings", "b_useShortcut", "False");
        

        private static string title_launcher = getValue("Custom", "title_launcher", "Launcher Title");
        private static string title_modpack = getValue("Custom", "title_modpack", "Modpack Name");
        private static string path_skyrim = getValue("path", "path_skyrim", "");
        private static string path_modpack = getValue("path", "path_modpack", "");
        private static string path_screenshot = getValue("path", "path_screenshot", "%modpackdir%%add_base_dir%\\overwrite\\root");
        private static string add_base_dir = getValue("Special", "add_base_dir", "");


        public static string Get_title_launcher()
        {
            return title_launcher;
        }
        public static string Get_title_modpack()
        {
            return title_modpack;
        }

        public static void Set_useExperimental(bool value)
        {
            try
            {
                b_useExperimental = value;
                changeValue("Settings", "b_useExperimental", value.ToString());
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"{ex.Message}");
            }
        }
        public static bool Get_useExperimental()
        {
            return b_useExperimental;
        }

        public static void Set_isFirstStartup(bool value)
        {
            try
            {
                b_isFirstStartup = value;
                changeValue("Settings", "b_isFirstStartup", value.ToString());
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"{ex.Message}");
            }

        }
        public static bool Get_isFirstStartup()
        {
            return b_isFirstStartup;
        }

        public static void Set_useOnline(bool value)
        {
            try
            {
                b_useOnline = value;
                changeValue("Settings", "b_useOnline", value.ToString());
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"{ex.Message}");
            }

        }
        public static bool Get_useOnline()
        {
            return b_useOnline;
        }

        public static void Set_path_skyrim(string value)
        {
            try
            {
                path_skyrim = value;
                changeValue("path", "path_skyrim", value);
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"{ex.Message}");

            }
        }
        public static string Get_path_skyrim()
        {
            return path_skyrim;
        }

        public static string Get_path_screenshot()
        {
            return path_screenshot;
        }

        public static void Set_path_modpack(string value)
        {
            try
            {
                path_modpack = value;
                changeValue("path", "path_modpack", value);
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"{ex.Message}");
            }
        }
        public static string Get_path_modpack()
        {
            return path_modpack;
        }

        public static string Get_add_base_dir()
        {
            return add_base_dir;
        }


        private static void Save()
        {
            fileIniDataParser.WriteFile(FileName, parsedData);

        }

        public static void changeValue(string header, string keyname, string value)
        {
            try
            {
                parsedData[header].GetKeyData(keyname).Value = value;
                Save();
            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"{ex.Message}");
            }

        }

        public static string getValue(string header, string keyname, string defaultvalue = "True")
        {
            string result = "";
            try
            {
                if (parsedData[header].ContainsKey(keyname))
                {
                    result = parsedData[header].GetKeyData(keyname).Value;
                } else
                {
                    KeyData newkey = new KeyData(keyname);
                    newkey.Value = defaultvalue;
                    parsedData[header].AddKey(newkey);
                    result = parsedData[header].GetKeyData(keyname).Value;
                }

            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"{ex.Message}");
            }

            return result;
        }

        public static bool getValue_bool(string header, string keyname, string defaultvalue = "")
        {

            bool result = false;

            try
            {

                if (parsedData[header].ContainsKey(keyname))
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
                else
                {
                    KeyData newkey = new KeyData(keyname);
                    newkey.Value = defaultvalue;
                    parsedData[header].AddKey(newkey);
                    result = getValue_bool(header, keyname);
                }

                

            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"{ex.Message}");
            }

            return result;

        }

    }
}
