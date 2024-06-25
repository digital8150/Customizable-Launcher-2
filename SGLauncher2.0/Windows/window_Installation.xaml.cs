using HandyControl.Controls;
using IniParser;
using IniParser.Model;
using SGLauncher2._0.Classes;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SGLauncher2._0.Windows
{
    /// <summary>
    /// window_Installation.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 



    public partial class window_Installation : System.Windows.Window
    {
        private int v_pagenum = 1;
        private string skyrimdir = "";
        private string modpackdir = "";
        private string sksedir = "";

        public static System.Windows.Window downgradeguide = new window_downgrade();
        public window_Installation()
        {
            InitializeComponent();
            viewchanger_hideall();
            grid_Intro.Visibility = Visibility.Visible;
            Thread t_intro = new Thread(thread_intro);
            t_intro.IsBackground = true;
            t_intro.Start();

            this.Title = AppSettings.getValue("Custom", "title_launcher");
            title_modpack1.Text = AppSettings.getValue("Custom", "title_modpack");
            title_modpack2.Text = AppSettings.getValue("Custom", "title_modpack");
            title_modpack3.Text = AppSettings.getValue("Custom", "title_modpack");
            title_modpack4.Text = AppSettings.getValue("Custom", "title_modpack");
            title_launcher1.Text = AppSettings.getValue("Custom", "title_launcher");



            carousel_img1.Source = uri_to_source(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\img\\c_img1");
            carousel_img2.Source = uri_to_source(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\img\\c_img2");
            carousel_img3.Source = uri_to_source(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\img\\c_img3");
            carousel_img4.Source = uri_to_source(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\img\\c_img4");

        }
        public static BitmapImage uri_to_source(string uri)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uri);
            bitmap.EndInit();

            return bitmap;
        }


        //뷰체인저 기능
        private void viewchanger(object sender, RoutedEventArgs e)
        {
            viewchanger_hideall();
            switch (v_pagenum)
            {
                case 1:
                    v_pagenum = 2;
                    grid_preinstall.Visibility = Visibility.Visible;
                    btn_next.IsEnabled = false;
                    Thread t_preinstall = new Thread(thread_preinstall);
                    t_preinstall.IsBackground = true;
                    t_preinstall.Start();
                    break;

                case 2:
                    v_pagenum = 3;
                    grid_done.Visibility = Visibility.Visible;
                    break;

                case 3:
                    //런처구동
                    System.Windows.Window launcher_window = new window_launcher();
                    launcher_window.Show();
                    this.Close();
                    try
                    {
                        Classes.AppSettings.Set_isFirstStartup(false);
                        Classes.AppSettings.Set_path_modpack(modpackdir);
                        Classes.AppSettings.Set_path_skyrim(skyrimdir);
                    }
                    catch (Exception ex)
                    {
                        Growl.Error($"{ex.Message}");
                    }
                    break;

                case 4:
                    //취소 페이지 : 종료
                    Dispatcher.Invoke(() => { System.Windows.Application.Current.Shutdown(); });
                    break;
            }
        }
        private void viewchanger_hideall()
        {
            Dispatcher.Invoke(() =>
            {
                grid_Intro.Visibility = Visibility.Hidden;
                grid_done.Visibility = Visibility.Hidden;
                grid_preinstall.Visibility = Visibility.Hidden;
                grid_canceled.Visibility = Visibility.Hidden;
            });

        }
        private void viewchanger_canceled()
        {

            Dispatcher.Invoke(() =>
            {
                v_pagenum = 4;
                btn_next.IsEnabled = true;
                btn_next.Content = "종료";
                viewchanger_hideall();
                Dispatcher.Invoke(() => { grid_canceled.Visibility = Visibility.Visible; });
            });

        }

        //인트로 페이지의 기능
        private void thread_intro()
        {
            bool flag = false;

            try
            {

                if (!(Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Bethesda Softworks\\Skyrim Special Edition", "Installed Path", "설치되지 않았습니다") == null))
                {
                    skyrimdir = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Bethesda Softworks\\Skyrim Special Edition", "Installed Path", "설치되지 않았습니다").ToString();
                    sksedir = skyrimdir + "skse64_loader.exe";

                }
                else
                {
                    skyrimdir = "레지스트리에서 스카이림 경로를 찾지 못했습니다.";
                    flag = true;
                }

                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "ModOrganizer.ini"))
                {
                    modpackdir = AppDomain.CurrentDomain.BaseDirectory;
                }
                else
                {
                    modpackdir = "현재 런처 실행 경로에 ModOrganizer.ini 가 존재하지 않습니다.";
                    flag = true;
                }





            }
            catch (Exception ex)
            {
                Growl.Error($"{ex}");
            }

            Dispatcher.Invoke(() =>
            {
                textbox_skyrimdir.Text = skyrimdir;
                textbox_modpackdir.Text = modpackdir;
                textbox_sksedir.Text = sksedir;
            });

            if (flag)
            {
                Dispatcher.Invoke(() => { btn_next.IsEnabled = false; });
            }
            while (flag)
            {
                if (File.Exists(skyrimdir + "SkyrimSE.exe") && File.Exists(modpackdir + "ModOrganizer.ini"))
                {
                    flag = false;
                    Dispatcher.Invoke(() => { btn_next.IsEnabled = true; });
                }
                Thread.Sleep(150);
            }


        }

        private void btn_skyrimdir_click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
                folderBrowserDialog1.ShowDialog();
                if (!System.IO.File.Exists(folderBrowserDialog1.SelectedPath + "\\SkyrimSE.exe"))
                {
                    throw new Exception("올바르지 않은 스카이림 폴더입니다. 스카이림 폴더에는 SkyrimSE.exe가 존재해야 합니다.");
                }
                skyrimdir = folderBrowserDialog1.SelectedPath + "\\";
                sksedir = skyrimdir + "skse64_loader.exe";
                textbox_skyrimdir.Text = skyrimdir;
                textbox_sksedir.Text = sksedir;
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex.Message}");
            }


        }
        private void btn_sksedir_click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.ShowDialog();
                if (!openFileDialog1.FileName.Contains("skse64_loader.exe"))
                {
                    throw new Exception("skse의 파일이름은 skse64_loader.exe 여야 합니다.");
                }
                sksedir = openFileDialog1.FileName;
                textbox_sksedir.Text = sksedir;
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex.Message}");
            }

        }
        private void btn_modpackdir_click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog openFileDialog1 = new FolderBrowserDialog();
                openFileDialog1.ShowDialog();
                if (!System.IO.File.Exists(openFileDialog1.SelectedPath + "\\ModOrganizer.ini"))
                {
                    throw new Exception("올바르지 않은 모드팩 폴더입니다. 모드팩 폴더에는 ModOrganizer.ini가 존재해야 합니다.");
                }

                int iAscii = System.Text.Encoding.ASCII.GetByteCount(openFileDialog1.SelectedPath);
                int iUnicode = System.Text.Encoding.UTF8.GetByteCount(openFileDialog1.SelectedPath);
                if( iAscii != iUnicode){
                    throw new Exception("모드팩 경로에서 한글이 감지되었습니다. MO2는 한글 경로를 지원하지 않습니다.");
                }
                modpackdir = openFileDialog1.SelectedPath + "\\";
                textbox_modpackdir.Text = modpackdir;
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex.Message}");
            }
        }

        //preinstall 페이지 기능
        private void thread_preinstall()
        {

            Thread.Sleep(1000);

            bool isAE = false;
            bool isSpInstalled = false;


            //1. AE 다운그레이드 검사
            if (fileHelper.getFileVerson(skyrimdir + "SkyrimSE.exe") != "1.5.97.0") //SkyrimSE 버전 확인
            {
                Growl.Warning($"스카이림 버전이 1.5.97.0이 아님\n감지한 버전 : {fileHelper.getFileVerson(skyrimdir + "SkyrimSE.exe")}");
                isAE = true;
            }
            fileHelper.isFileExists(skyrimdir + "data\\ccBGSSSE001-Fish.esm", ref isAE);
            fileHelper.isFileExists(skyrimdir + "data\\ccBGSSSE025-AdvDSGS.esm", ref isAE);
            fileHelper.isFileExists(skyrimdir + "data\\ccBGSSSE037-Curios.esl", ref isAE);
            fileHelper.isFileExists(skyrimdir + "data\\ccQDRSSE001-SurvivalMode.esl", ref isAE);
            fileHelper.isFileExists(skyrimdir + "data\\_ResourcePack.esl", ref isAE);

            //UI 업데이트
            Dispatcher.Invoke(() => { spinner_SEDowngrade.Spin = false; });
            if (isAE)
            {
                //AE 일경우
                Dispatcher.Invoke(() =>
                {
                    spinner_SEDowngrade.Icon = FontAwesome6.EFontAwesomeIcon.Solid_TriangleExclamation;
                    spinner_SEDowngrade.Foreground = new SolidColorBrush(Color.FromRgb(0xff, 0x0, 0x0));
                    downgradeguide.Show();
                });

                
                

                while (true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        border_waiting.Visibility = Visibility.Visible;
                    });

                    isAE = false;

                    if (fileHelper.getFileVerson(skyrimdir + "SkyrimSE.exe") != "1.5.97.0") //SkyrimSE 버전 확인
                    {
                        
                        isAE = true;
                    }
                    fileHelper.isFileExistssilent(skyrimdir + "data\\ccBGSSSE001-Fish.esm", ref isAE);
                    fileHelper.isFileExistssilent(skyrimdir + "data\\ccBGSSSE025-AdvDSGS.esm", ref isAE);
                    fileHelper.isFileExistssilent(skyrimdir + "data\\ccBGSSSE037-Curios.esl", ref isAE);
                    fileHelper.isFileExistssilent(skyrimdir + "data\\ccQDRSSE001-SurvivalMode.esl", ref isAE);
                    fileHelper.isFileExistssilent(skyrimdir + "data\\_ResourcePack.esl", ref isAE);

                    if (!isAE)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            border_waiting.Visibility = Visibility.Collapsed;
                        });
                        break;
                    }
                    Thread.Sleep(1000);
                }
                /**
                //동의 구하고 다운그레이드
                if (HandyControl.Controls.MessageBox.Show("SkyrimAE가 설치되어 있습니다. SE로 다운그레이드 하시겠습니까?", null, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Dispatcher.Invoke(() =>
                    {
                        spinner_SEDowngrade.Icon = FontAwesome6.EFontAwesomeIcon.Solid_Spinner;
                        spinner_SEDowngrade.Foreground = new SolidColorBrush(Color.FromRgb(0x0, 0x0, 0x0));
                        spinner_SEDowngrade.Spin = true;
                    });


                    try
                    {
                        //파일 삭제
                        File.Delete(skyrimdir + "data\\ccBGSSSE001-Fish.esm");
                        File.Delete(skyrimdir + "data\\ccBGSSSE025-AdvDSGS.esm");
                        File.Delete(skyrimdir + "data\\ccBGSSSE037-Curios.esl");
                        File.Delete(skyrimdir + "data\\ccQDRSSE001-SurvivalMode.esl");

                        //스타터팩 설치
                        fileHelper.unZip("skyrimdir.zip", skyrimdir);

                    }
                    catch (Exception ex)
                    {
                        //오류 발생시 취소
                        Growl.Error($"AE컨텐츠 삭제 중 오류 발생 : {ex.Message}");
                        HandyControl.Controls.MessageBox.Show("AE컨텐츠를 삭제하는 중 문제가 발생하여 삭제하지 못했습니다.\n게임 또는 영향을 줄만한 다른 프로그램을 종료하고 다시 시도해주세요.");
                        viewchanger_canceled();
                        return;

                    }
                }
                else
                {
                    //미 동의 시 취소
                    viewchanger_canceled();
                    return;
                }
                **/

            }

            Dispatcher.Invoke(() =>
            {
                spinner_SEDowngrade.Spin = false;
                spinner_SEDowngrade.Icon = FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck;
                spinner_SEDowngrade.Foreground = new SolidColorBrush(Color.FromRgb(0x0, 0xff, 0x0));
            });



            //2. 스타터팩 설치 검사
            //파일 존재 여부 검사
            isSpInstalled = fileHelper.checkZip("skyrimdir.zip", skyrimdir);

            //enb 버전 검사
            try
            {
                if (File.Exists(skyrimdir + "d3d11.dll"))
                {
                    if (System.Int32.Parse(fileHelper.getFileVerson(skyrimdir + "d3d11.dll").Replace(", ", "")) < 488)
                    {
                        Growl.Warning($"ENB Binary 버전이 {fileHelper.getFileVerson(skyrimdir + "d3d11.dll")} 으로 모드팩 최소 요구버전 0.488 버전보다 낮습니다.");
                        isSpInstalled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex.Message}");
            }

            if (!isSpInstalled)
            {
                //스타터팩 설치가 안됨
                //동의 받고 설치
                Dispatcher.Invoke(() =>
                {
                    spinner_SPinstal.Spin = false;
                    spinner_SPinstal.Icon = FontAwesome6.EFontAwesomeIcon.Solid_TriangleExclamation;
                    spinner_SPinstal.Foreground = new SolidColorBrush(Color.FromRgb(0xff, 0x0, 0x0));
                });
                if (HandyControl.Controls.MessageBox.Show("스타터팩 설치가 필요합니다. 설치하시겠습니까?", null, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    //스타터팩 설치
                    Dispatcher.Invoke(() =>
                    {
                        spinner_SPinstal.Icon = FontAwesome6.EFontAwesomeIcon.Solid_Spinner;
                        spinner_SPinstal.Foreground = new SolidColorBrush(Color.FromRgb(0x0, 0x0, 0x0));
                        spinner_SPinstal.Spin = true;
                    });

                    fileHelper.unZip("skyrimdir.zip", skyrimdir);
                }
                else
                {
                    //미 동의 시 취소
                    viewchanger_canceled();
                    return;
                }

            } //스타터팩 미설치 시 동작

            Dispatcher.Invoke(() =>
            {
                spinner_SPinstal.Spin = false;
                spinner_SPinstal.Icon = FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck;
                spinner_SPinstal.Foreground = new SolidColorBrush(Color.FromRgb(0x0, 0xff, 0x0));
            });

            //3. moini 설정 적용
            try
            {
                string str = modpackdir.Replace("\\", "/");
                str = str.Substring(0, str.Length - 1);
                FileIniDataParser fileIniDataParser = new FileIniDataParser();
                IniData parsedData = fileIniDataParser.ReadFile(modpackdir + "ModOrganizer.ini");
                parsedData["Settings"].GetKeyData("base_directory").Value = str + AppSettings.Get_add_base_dir().Replace("\\", "/");
                int num = int.Parse(parsedData["customExecutables"].GetKeyData("size").Value);
                for (int index = 1; index < num; ++index)
                {
                    string keyName = string.Format("{0}\\binary", (object)index);
                    KeyData keyData = parsedData["customExecutables"].GetKeyData(keyName);
                    if (keyData.Value.Contains("/mods"))
                    {
                        int startIndex = keyData.Value.IndexOf("/mods");
                        keyData.Value = str + AppSettings.Get_add_base_dir().Replace("\\", "/") + keyData.Value.Substring(startIndex);
                    }
                    else if (keyData.Value.Contains("/Tools"))
                    {
                        int startIndex = keyData.Value.IndexOf("/Tools");
                        keyData.Value = str + keyData.Value.Substring(startIndex);
                    }
                    else if (keyData.Value.Contains("skse64_loader"))
                        keyData.Value = skyrimdir.Replace("\\", "/") + "skse64_loader.exe";
                }
                fileIniDataParser.WriteFile(modpackdir + "ModOrganizer.ini", parsedData);

            }
            catch (Exception ex)
            {
                Growl.Error($"{ex.Message}");
                viewchanger_canceled();
                return;
            }

            Dispatcher.Invoke(() =>
            {
                spinner_moiniapply.Spin = false;
                spinner_moiniapply.Icon = FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck;
                spinner_moiniapply.Foreground = new SolidColorBrush(Color.FromRgb(0x0, 0xff, 0x0));
                btn_next.IsEnabled = true;
            });






        }
    }

    public class fileHelper
    {
        public static string getFileVerson(string filepath)
        {
            try
            {
                if (System.IO.File.Exists(filepath))
                {
                    FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(filepath);
                    string versionString = fileVersionInfo.ProductVersion;
                    return versionString;
                }
                else
                {
                    return "버전읽기 오류, 파일이 존재하지 않습니다. " + filepath;
                }
            }
            catch (Exception ex)
            {

                return $"버전읽기 오류 : {ex.Message}";
            }

        }
        public static void isFileExists(string filepath, ref bool flag)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    flag = true;
                    Growl.Info($"AE컨텐츠 발견 : {Path.GetFileName(filepath)}");
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex.Message}");
            }
        }
        public static void isFileExistssilent(string filepath, ref bool flag)
        {
            try
            {
                if (File.Exists(filepath))
                {
                    flag = true;
                    
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"{ex.Message}");
            }
        }

        //압축관련
        public static void unZip(string zipfilepath, string extractdir)
        {
            string zipPath = zipfilepath;
            string extractPath = extractdir;

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string entryFullname = System.IO.Path.Combine(extractPath, entry.FullName);
                        string enryPath = System.IO.Path.GetDirectoryName(entryFullname);
                        if (!Directory.Exists(enryPath))
                        {
                            Directory.CreateDirectory(enryPath);
                        }

                        string entryFn = System.IO.Path.GetFileName(entryFullname);
                        if (!string.IsNullOrEmpty(entryFn))
                        {
                            entry.ExtractToFile(entryFullname, true);
                        }




                    }
                }

            }
            catch (Exception ex)
            {
                Growl.Error($"압축 해제 중 오류 발생 : {ex.Message}");
            }



        }

        public static void unZip(string zipfilepath, string extractdir, ref int progress)
        {
            string zipPath = zipfilepath;
            string extractPath = extractdir;

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {

                    int index = 1;
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string entryFullname = System.IO.Path.Combine(extractPath, entry.FullName);
                        string enryPath = System.IO.Path.GetDirectoryName(entryFullname);
                        if (!Directory.Exists(enryPath))
                        {
                            Directory.CreateDirectory(enryPath);
                        }

                        string entryFn = System.IO.Path.GetFileName(entryFullname);
                        if (!string.IsNullOrEmpty(entryFn))
                        {
                            entry.ExtractToFile(entryFullname, true);
                            progress = (int)((double)index / (double)archive.Entries.Count) * 100;
                        }



                        index++;
                    }
                }

            }
            catch (Exception ex)
            {
                Growl.Error($"압축 해제 중 오류 발생 : {ex.Message}");
            }



        }

        public static bool checkZip(string zipfilepath, string targetdir) //true : 스타터팩이 올바름 false: 스타터팩 결점있음
        {
            string zipPath = zipfilepath;
            string extractPath = targetdir;
            bool result = true;
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string entryFullname = System.IO.Path.Combine(extractPath, entry.FullName);
                        string enryPath = System.IO.Path.GetDirectoryName(entryFullname);
                        if (!Directory.Exists(enryPath))
                        {
                            Directory.CreateDirectory(enryPath);
                        }

                        string entryFn = System.IO.Path.GetFileName(entryFullname);
                        if (!string.IsNullOrEmpty(entryFn))
                        {
                            if (!File.Exists(entryFullname))
                            {
                                result = false;
                                Growl.Info($"{entryFullname} 미존재");
                                return result;
                            }
                        }




                    }
                }

            }
            catch (Exception ex)
            {
                Growl.Error($"스타터팩 설치 점검 중 오류 발생 : {ex.Message}");
            }
            return result;

        }
    }


}

