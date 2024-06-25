using HandyControl.Controls;
using System;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Net.Http;

namespace SGLauncher2._0.Classes
{
    internal class AppUpdate
    {

        private static string PATH_MODPACKVERSION = $"{AppSettings.Get_path_modpack()}{AppSettings.getValue("Updater", "file_version")}";
        private static string API_VERSION = AppSettings.getValue("Updater", "auth_api") + "get_version.php?identifier=";
        private static string API_URL = AppSettings.getValue("Updater", "auth_api") + "get_url.php?identifier=";
        private static string API_NAME = AppSettings.getValue("Updater", "auth_name");
        private static HttpClient httpClient = new HttpClient();


        public static string modpack_version_current = getModpackVersionCurrent();
        public static string modpack_version_latest = null;
        public static string update_url = null;
        public static bool? isUptodate = null;



        public static void updateModpackVersionCurrent()
        {
            modpack_version_current = getModpackVersionCurrent();
        }
        public static string getModpackVersionCurrent()
        {

            string result = null;
            try
            {
                result = File.ReadAllText(PATH_MODPACKVERSION);

            }
            catch (Exception ex)
            {
                Growl.Error($"모드팩 버전 읽기 실패 : {ex.Message} {PATH_MODPACKVERSION}");
            }

            return result;

        }


        public static async void retriveVersions()
        {

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(API_VERSION + API_NAME);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                modpack_version_latest = responseBody;
                response = await httpClient.GetAsync(API_URL + API_NAME);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();
                update_url = responseBody;

                if(modpack_version_current != modpack_version_latest)
                {
                    isUptodate = false;
                }
                else
                {
                    isUptodate = true;
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"온라인 기능 실패 : {ex.Message}\n온라인 서버가 더 이상 작동하지 않는다면 런처 환경설정에서 온라인 기능을 비활성화 할 수 있습니다.");
            }


        }

    }
}
