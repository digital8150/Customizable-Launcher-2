using HandyControl.Controls;
using Microsoft.Win32;
using SGLauncher2._0.Classes;
using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SGLauncher2._0.Windows
{
    /// <summary>
    /// window_troubleshoot.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class window_troubleshoot : System.Windows.Window
    {
        private bool flag_global = false;
        private bool flag_fasoo = false;
        private bool flag_AVX = false;
        private bool flag_ENB = false;
        private bool flag_skyrimVersion = false;
        private bool flag_starterPack = false;
        private bool flag_windowsBuild = false;

        private System.Windows.Window downgradeguide2 = new window_downgrade();

        public window_troubleshoot()
        {
            InitializeComponent();
            border_main.Height = 0;

            if (!AppSettings.getValue_bool("Diagnosis", "check_fasoo")) grid_fasoo.Visibility = Visibility.Collapsed;
            if (!AppSettings.getValue_bool("Diagnosis", "check_avx")) grid_avx.Visibility = Visibility.Collapsed;
            if (!AppSettings.getValue_bool("Diagnosis", "check_enb")) grid_enb.Visibility = Visibility.Collapsed;
            if (!AppSettings.getValue_bool("Diagnosis", "check_sse")) grid_sse.Visibility = Visibility.Collapsed;
            if (!AppSettings.getValue_bool("Diagnosis", "check_starterpack")) grid_starterpack.Visibility = Visibility.Collapsed;
            if (!AppSettings.getValue_bool("Diagnosis", "check_winver")) grid_winver.Visibility = Visibility.Collapsed;
            textblock_targetversion.Text = AppSettings.getValue("Diagnosis", "enb_targetversion");

            this.Title = AppSettings.Get_title_launcher();
        }

        //Window Features
        //Dragmove feature
        private void window_dragmove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        //Window open transition
        private void Window_loaded(object sender, RoutedEventArgs e)
        {

            Thread animationThread = new Thread(() =>
            {


                Dispatcher.Invoke(() =>
                {
                    DoubleAnimation animation = new DoubleAnimation();
                    animation.From = 0;
                    animation.To = 720;
                    animation.Duration = TimeSpan.FromSeconds(0.25);
                    animation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
                    border_main.BeginAnimation(HeightProperty, animation);
                });
            }); animationThread.Start();
        }

        //close button
        private void btn_close_click(object sender, RoutedEventArgs e)
        {
            //설정 화면 닫고 복귀
            System.Windows.Window launcherwindow = new window_launcher();
            launcherwindow.Show();
            this.Close();
        }

        //진단 시작 버튼
        private void btn_diagnoise_click(object sender, RoutedEventArgs e)
        {
            if (flag_global)
            {
                Thread thread = new Thread(fix);
                thread.IsBackground = true;
                thread.Start();
            }
            else
            {
                Thread thread = new Thread(diagnoise);
                thread.IsBackground = true;
                thread.Start();
            }

        }

        //인디케이터 제어함수
        private void set_indicator(ref FontAwesome6.Fonts.FontAwesome indicator, FontAwesome6.EFontAwesomeIcon Icon, SolidColorBrush Color, bool isSpin)
        {
            indicator.Icon = Icon;
            indicator.Spin = isSpin;
            indicator.Foreground = Color;
        }

        //진단스레드
        private void diagnoise()
        {
            //UI 업데이트
            Dispatcher.Invoke(() =>
            {
                set_indicator(ref btn_indicator, FontAwesome6.EFontAwesomeIcon.Solid_Spinner, new SolidColorBrush(Colors.Black), true);
                set_indicator(ref fasoo_indicator, FontAwesome6.EFontAwesomeIcon.Solid_Spinner, new SolidColorBrush(Colors.Black), true);
                set_indicator(ref avx_indicator, FontAwesome6.EFontAwesomeIcon.Solid_Spinner, new SolidColorBrush(Colors.Black), true);
                set_indicator(ref ae_indicator, FontAwesome6.EFontAwesomeIcon.Solid_Spinner, new SolidColorBrush(Colors.Black), true);
                set_indicator(ref enb_indicator, FontAwesome6.EFontAwesomeIcon.Solid_Spinner, new SolidColorBrush(Colors.Black), true);
                set_indicator(ref sp_indicator, FontAwesome6.EFontAwesomeIcon.Solid_Spinner, new SolidColorBrush(Colors.Black), true);
                set_indicator(ref win_indicator, FontAwesome6.EFontAwesomeIcon.Solid_Spinner, new SolidColorBrush(Colors.Black), true);


                btn_text.Text = "진단중";
                btn_diagnoise.IsEnabled = false;
            });

            //Fasoo DRM 체크

            if (!AppSettings.getValue_bool("Diagnosis", "check_fasoo")) goto AVX;

            try
            {
                string fasooinstalldir = "";
                try
                {
                    fasooinstalldir = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Fasoo DRM", "InstallPath", "설치되지 않음").ToString();
                }
                catch
                {
                    fasooinstalldir = "설치되지 않음";
                }

                if (!File.Exists(fasooinstalldir + "\\fph.exe"))
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref fasoo_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck, new SolidColorBrush(Colors.Green), false);
                        fasoo_result.Text = $"검사 결과 : FasooDRM 설치경로({fasooinstalldir})";

                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref fasoo_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleExclamation, new SolidColorBrush(Colors.Red), false);
                        fasoo_result.Text = $"검사 결과 : FasooDRM 설치경로({fasooinstalldir})";
                        flag_global = true;
                        flag_fasoo = true;
                    });
                }

            }
            catch (Exception ex)
            {
                Growl.Error($"Fasoo 검사 중 예외 발생 : {ex.Message}");
            }

            AVX:
            if (!AppSettings.getValue_bool("Diagnosis", "check_avx")) goto ENB;
            //AVX 체크
            try
            {
                bool isAVXSupported = AVXSupportChecker.IsAVXSupported();
                string result = $"검사 결과 : {AVXSupportChecker.GetCPUName()} , isAVXSupported : {AVXSupportChecker.IsAVXSupported()}";
                if (isAVXSupported)
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref avx_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck, new SolidColorBrush(Colors.Green), false);

                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref avx_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleExclamation, new SolidColorBrush(Colors.Red), false);
                        flag_global = true;
                        flag_AVX = true;
                    });
                }
                Dispatcher.Invoke(() =>
                {
                    avx_result.Text = result;
                });

            }
            catch (Exception ex)
            {
                Growl.Error($"AVX 명령어 확인 중 예외 발생 : {ex.Message}");
            }

            ENB:
            if (!AppSettings.getValue_bool("Diagnosis", "check_enb")) goto SSE;
            //ENB 바이너리 체크
            try
            {
                if (File.Exists(AppSettings.Get_path_skyrim() + "d3d11.dll"))
                {
                    int targetversion = 0;
                    try
                    {
                        targetversion = System.Int32.Parse(AppSettings.getValue("Diagnosis", "enb_targetversion"));
                    }
                    catch (Exception ex)
                    {
                        Growl.Error($"런처 설정파일의 enb_targetversion에 문제가 있습니다. 정수가 아닌 것 같습니다. {ex.Message}");
                    }
                    if (System.Int32.Parse(fileHelper.getFileVerson(AppSettings.Get_path_skyrim() + "d3d11.dll").Replace(", ", "")) < targetversion)
                    {
                        Growl.Warning($"ENB Binary 버전이 {fileHelper.getFileVerson(AppSettings.Get_path_skyrim() + "d3d11.dll")} 으로 모드팩 최소 요구버전 0.{targetversion} 버전보다 낮습니다.");
                        Dispatcher.Invoke(() =>
                        {
                            set_indicator(ref enb_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleExclamation, new SolidColorBrush(Colors.Red), false);
                            flag_global = true;
                            flag_ENB = true;
                        });

                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            set_indicator(ref enb_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck, new SolidColorBrush(Colors.Green), false);
                        });

                    }
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref enb_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleExclamation, new SolidColorBrush(Colors.Red), false);
                        flag_global = true;
                        flag_ENB = true;
                    });

                }

                Dispatcher.Invoke(() =>
                {
                    enb_result.Text = $"검사 결과 : {fileHelper.getFileVerson(AppSettings.Get_path_skyrim() + "d3d11.dll")}";
                });
            }
            catch (Exception ex)
            {
                Growl.Error($"ENB 바이너리 검사 중 예외 발생 {ex.Message}");
            }

            SSE:
            if (!AppSettings.getValue_bool("Diagnosis", "check_sse")) goto STARTERPACK;
            //스카이림 버전 검사
            try
            {
                bool isAE = false;



                //1. AE 다운그레이드 검사
                if (fileHelper.getFileVerson(AppSettings.Get_path_skyrim() + "SkyrimSE.exe") != "1.5.97.0") //SkyrimSE 버전 확인
                {
                    Growl.Warning($"스카이림 버전이 1.5.97.0이 아님\n감지한 버전 : {fileHelper.getFileVerson(AppSettings.Get_path_skyrim() + "SkyrimSE.exe")}");
                    isAE = true;
                }
                fileHelper.isFileExists(AppSettings.Get_path_skyrim() + "data\\ccBGSSSE001-Fish.esm", ref isAE);
                fileHelper.isFileExists(AppSettings.Get_path_skyrim() + "data\\ccBGSSSE025-AdvDSGS.esm", ref isAE);
                fileHelper.isFileExists(AppSettings.Get_path_skyrim() + "data\\ccBGSSSE037-Curios.esl", ref isAE);
                fileHelper.isFileExists(AppSettings.Get_path_skyrim() + "data\\ccQDRSSE001-SurvivalMode.esl", ref isAE);

                if (isAE)
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref ae_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleExclamation, new SolidColorBrush(Colors.Red), false);
                        flag_global = true;
                        flag_skyrimVersion = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref ae_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck, new SolidColorBrush(Colors.Green), false);
                    });
                }

                Dispatcher.Invoke(() =>
                {
                    ae_result.Text = $"검사 결과 : 스카이림 버전({fileHelper.getFileVerson(AppSettings.Get_path_skyrim() + "SkyrimSE.exe")}),\nisAE({isAE})";
                });
            }
            catch (Exception ex)
            {
                Growl.Error($"스카이림 버전 검사 중 예외 발생 {ex.Message}");
            }

            STARTERPACK:
            if (!AppSettings.getValue_bool("Diagnosis", "check_starterpack")) goto WINVER;
            //스타터팩 무결성 검사
            try
            {
                bool isSpInstalled = fileHelper.checkZip("skyrimdir.zip", AppSettings.Get_path_skyrim());
                if (isSpInstalled)
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref sp_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck, new SolidColorBrush(Colors.Green), false);
                    });

                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref sp_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleExclamation, new SolidColorBrush(Colors.Red), false);
                        flag_global = true;
                        flag_starterPack = true;
                    });

                }
                Dispatcher.Invoke(() =>
                {
                    sp_result.Text = $"검사 결과 : 스타터팩 무결성 여부({isSpInstalled})";
                });

            }
            catch (Exception ex)
            {
                Growl.Error($"스타터팩 검사 중 예외 발생 {ex.Message}");
            }

            WINVER:
            if (!AppSettings.getValue_bool("Diagnosis", "check_winver")) goto FINISH;
            //윈도우 빌드 버전 검사
            try
            {
                if (WindowsBuildChecker.IsWindowsBuild18312Under())
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref win_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleExclamation, new SolidColorBrush(Colors.Red), false);
                        flag_global = true;
                        flag_windowsBuild = true;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        set_indicator(ref win_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck, new SolidColorBrush(Colors.Green), false);
                    });
                }
                Dispatcher.Invoke(() =>
                {
                    win_result.Text = $"검사 결과 : {WindowsBuildChecker.GetWindowsBuild()}";
                });

            }
            catch (Exception ex)
            {
                Growl.Error($"빌드 검사 중 예외 발생 {ex.Message}");
            }

            FINISH:
            //UI 업데이트
            Dispatcher.Invoke(() =>
            {
                if (flag_global)
                {
                    set_indicator(ref btn_indicator, FontAwesome6.EFontAwesomeIcon.Solid_WandMagicSparkles, new SolidColorBrush(Colors.Black), false);
                    btn_text.Text = "오류 수정";
                    btn_diagnoise.IsEnabled = true;
                    btn_dse.Color = Colors.Red;
                }
                else
                {
                    set_indicator(ref btn_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck, new SolidColorBrush(Colors.Black), false);
                    btn_text.Text = "문제가 검출되지 않음";
                    btn_dse.Opacity = 0;
                    btn_diagnoise.IsEnabled = false;
                }


            });
        }

        //수정 스레드
        private void fix()
        {
            flag_global = false;

            //UI 업데이트
            Dispatcher.Invoke(() =>
            {
                set_indicator(ref btn_indicator, FontAwesome6.EFontAwesomeIcon.Solid_Spinner, new SolidColorBrush(Colors.Black), true);
                btn_text.Text = "수정 중";
                btn_diagnoise.IsEnabled = false;
            });

            //파수 수정
            if (flag_fasoo)
            {
                HandyControl.Controls.MessageBox.Show("Fasoo DRM은 기업용 보안 제품으로, 런처가 자동으로 삭제하는 것이 제한됩니다. 프로그램 추가/제거 화면에서 Fasoo DRM을 찾아 삭제해주십시오.");
                Process.Start("appwiz.cpl");
                HandyControl.Controls.MessageBox.Show("Fasoo 삭제 완료 후 이 대화창을 닫아주세요.");
                while (AppManager.isFasooInstalled())
                {
                    HandyControl.Controls.MessageBox.Show("Fasoo가 삭제되지 않은 것 같습니다. 삭제 완료 후 이 대화창을 닫아주세요.");
                }
                flag_fasoo = false;


            }

            // AVX 수정 안내문
            if (flag_AVX)
            {
                HandyControl.Controls.MessageBox.Show("런처가 AVX 명령어 미지원 문제점을 확인했으나, 기술적 제한사항으로 수정사항을 직접 적용할 수 없습니다. AVX는 샌디브릿지(인텔 코어 2세대) 에서 최초 지원되기 시작한 명령어 집합입니다. 프로세서를 업그레이드 하거나 HDT_SMP 와 SmoothCam 모드를 AVX 미 사용 버전으로 재설치 해주십시오.");
            }

            //ENB 바이너리, 스타터팩 수정, AE 다운그레이드
            if (flag_ENB || flag_starterPack)
            {
                fileHelper.unZip("skyrimdir.zip", AppSettings.Get_path_skyrim());
            }

            if (flag_skyrimVersion)
            {
                Dispatcher.Invoke(() =>
                {
                    downgradeguide2.Show();
                });

            }
            flag_skyrimVersion = false;
            flag_ENB = false;
            flag_starterPack = false;

            //윈도우 빌드 안내문
            if (flag_windowsBuild)
            {
                HandyControl.Controls.MessageBox.Show("윈도우를 업데이트 해주세요.");
                Process.Start("ms-settings:windowsupdate");
            }
            flag_windowsBuild = false;

            //UI 업데이트
            Dispatcher.Invoke(() =>
            {
                set_indicator(ref btn_indicator, FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck, new SolidColorBrush(Colors.Black), false);
                btn_text.Text = "수정 완료!";
                btn_diagnoise.IsEnabled = true;

            });
            diagnoise();

        }
    }

    public class AVXSupportChecker
    {
        public static bool IsAVXSupported()
        {
            bool isSupported = false;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                try
                {
                    // Windows에서 AVX 지원 확인
                    isSupported = IsAVXSupportedWindows();
                }
                catch (Exception ex)
                {
                    // 예외 처리
                    isSupported = false;
                    Growl.Error($"AVX 지원 확인 중 예외 발생 : {ex.Message}");
                }
            }
            else
            {
                // Windows 이외의 플랫폼에서는 지원하지 않음으로 반환
                isSupported = false;
            }

            return isSupported;
        }

        private static bool IsAVXSupportedWindows()
        {
            if (IsProcessorFeaturePresent(ProcessorFeature.PF_XMMI64_INSTRUCTIONS_AVAILABLE) &&
                IsProcessorFeaturePresent(ProcessorFeature.PF_AVX_SUPPORT) &&
                IsProcessorFeaturePresent(ProcessorFeature.PF_AVX2_SUPPORT))
            {
                return true;
            }

            return false;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsProcessorFeaturePresent(ProcessorFeature processorFeature);

        private enum ProcessorFeature
        {
            PF_XMMI64_INSTRUCTIONS_AVAILABLE = 10, // SSE2
            PF_AVX_SUPPORT = 12,
            PF_AVX2_SUPPORT = 13,
        }

        public static string GetCPUName()
        {
            try
            {
                // WMI 쿼리를 사용하여 CPU 정보를 가져옵니다.
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_Processor");
                ManagementObjectCollection collection = searcher.Get();

                foreach (ManagementObject obj in collection)
                {
                    // CPU 모델 이름 속성을 가져와서 반환합니다.
                    string cpuName = obj["Name"].ToString();
                    return cpuName;
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                Growl.Error("CPU 정보를 가져오는 동안 오류 발생: " + ex.Message);
            }

            // CPU 정보를 가져올 수 없는 경우 빈 문자열 반환
            return "";
        }
    }

    public class WindowsBuildChecker
    {
        public static string GetWindowsBuild()
        {
            string result = "";
            try
            {
                // Windows 레지스트리에서 CurrentBuildNumber 값을 가져옵니다.
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        result = $"{key.GetValue("CurrentBuildNumber")?.ToString()}";

                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                Growl.Error("Windows 빌드 번호를 가져오는 동안 오류 발생: " + ex.Message);
                result = $"Windows 빌드 번호를 가져오는 동안 오류 발생";
            }

            return result;

        }

        public static bool IsWindowsBuild18312Under()
        {
            try
            {
                // Windows 레지스트리에서 CurrentBuildNumber 값을 가져옵니다.
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        string buildNumberString = key.GetValue("CurrentBuildNumber")?.ToString();
                        if (!string.IsNullOrEmpty(buildNumberString) && int.TryParse(buildNumberString, out int buildNumber))
                        {
                            // 현재 빌드 번호가 18312 이상인지 확인합니다.
                            if (buildNumber < 18312)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 처리
                Growl.Error("Windows 빌드 번호를 가져오는 동안 오류 발생: " + ex.Message);
            }

            // 18312 이상인 경우나 정보를 가져올 수 없는 경우 false 반환
            return false;
        }
    }

    public class AppManager
    {
        public static bool UninstallProgram(string ProgramName)
        {
            try
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher(
                  "SELECT * FROM Win32_Product WHERE Name = '" + ProgramName + "'");
                foreach (ManagementObject mo in mos.Get())
                {
                    try
                    {
                        if (mo["Name"].ToString() == ProgramName)
                        {
                            object hr = mo.InvokeMethod("Uninstall", null);
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        //this program may not have a name property, so an exception will be thrown
                        Growl.Error($"AppManager_exception : {ex.Message}");
                    }
                }

                //was not found...
                return false;

            }
            catch (Exception ex)
            {
                Growl.Error($"AppManager_exception : {ex.Message}");
                return false;
            }
        }

        public static string FindUninstallString(string programNameContains)
        {
            string uninstallString = null;

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                if (key != null)
                {
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        using (RegistryKey subKey = key.OpenSubKey(subKeyName))
                        {

                            object displayNameValue = subKey.GetValue("DisplayName");

                            if (displayNameValue != null && displayNameValue.ToString().Contains(programNameContains))
                            {
                                uninstallString = subKey.GetValue("UninstallString") as string;
                                break;
                            }
                        }
                    }
                }
            }

            return uninstallString;
        }

        public static void RunUninstallString(string uninstallString)
        {
            if (!string.IsNullOrEmpty(uninstallString))
            {
                Process process = new Process();
                process.StartInfo.FileName = uninstallString;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                process.WaitForExit();
            }
        }

        public static bool isFasooInstalled()
        {
            bool? result = null;
            try
            {
                string fasooinstalldir = "";
                try
                {
                    fasooinstalldir = Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Fasoo DRM", "InstallPath", "설치되지 않음").ToString();
                }
                catch
                {
                    fasooinstalldir = "설치되지 않음";
                }

                if (!File.Exists(fasooinstalldir + "\\fph.exe"))
                {
                    result = false;
                }
                else
                {
                    result = true;
                }

            }
            catch (Exception ex)
            {
                Growl.Error($"Fasoo 검사 중 예외 발생 : {ex.Message}");
            }

            return (bool)result;
        }
    }
}
