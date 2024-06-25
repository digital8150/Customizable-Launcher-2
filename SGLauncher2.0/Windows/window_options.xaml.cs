using HandyControl.Controls;
using SGLauncher2._0.Classes;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace SGLauncher2._0.Windows
{
    /// <summary>
    /// window_options.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class window_options : System.Windows.Window
    {
        private static iniFileHelper moiniReader = new iniFileHelper();
        private static iniFileHelper settingsReader = new iniFileHelper();
        private static iniFileHelper dptReader = new iniFileHelper();

        public window_options()
        {
            InitializeComponent();
            border_main.Height = 0;
            viewchanger_hideall();
            page_video.Visibility = Visibility.Visible;
            moiniReader = new iniFileHelper($"{AppSettings.Get_path_modpack()}ModOrganizer.ini");
            settingsReader = new iniFileHelper($"{AppSettings.Get_path_modpack().Substring(0, AppSettings.Get_path_modpack().Length - 1)}{AppSettings.Get_add_base_dir()}\\profiles\\{Regex.Match(moiniReader.getValue("General", "selected_profile"), @"\@ByteArray\((.*?)\)").Groups[1].Value}\\skyrimprefs.ini");
            dptReader = new iniFileHelper($"{AppSettings.Get_path_modpack().Substring(0, AppSettings.Get_path_modpack().Length - 1)}{AppSettings.Get_add_base_dir()}{AppSettings.getValue("Options", "ini_displaytweak")}");

            this.Title = AppSettings.Get_title_launcher();
            ImageBrush imgb = new ImageBrush();
            imgb.ImageSource = uri_to_source(AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\img\\background");
            border_main.Background = imgb;
        }
        public static BitmapImage uri_to_source(string uri)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(uri);
            bitmap.EndInit();

            return bitmap;
        }


        private void viewchanger(object sender, RoutedEventArgs e)
        {
            viewchanger_hideall();
            if (vc_gameplay.IsChecked == true)
            {
                Dispatcher.Invoke(() => { page_gameplay.Visibility = Visibility.Visible; });
            }
            else if (vc_video.IsChecked == true)
            {
                Dispatcher.Invoke(() => { page_video.Visibility = Visibility.Visible; });
            }
            else if (vc_launcher.IsChecked == true)
            {
                Dispatcher.Invoke(() => { page_launcher.Visibility = Visibility.Visible; });
            }
        }

        //뷰체인저 기능
        private void viewchanger_hideall()
        {
            Dispatcher.Invoke(() =>
            {
                page_video.Visibility = Visibility.Hidden;
                page_gameplay.Visibility = Visibility.Hidden;
                page_launcher.Visibility = Visibility.Hidden;
            });


        }


        //Window Features
        //Dragmove feature
        private void window_dragmove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        //Window open transition & settingsReader
        private void Window_loaded(object sender, RoutedEventArgs e)
        {

            DoubleAnimation animation = new DoubleAnimation();

            animation.From = 0;
            animation.To = 720;
            animation.Duration = TimeSpan.FromSeconds(0.25);
            animation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };


            border_main.BeginAnimation(HeightProperty, animation);

            //설정값을 읽어와 화면에 최신화
            try
            {

                if (settingsReader.getValue("Display", "bFull Screen") == "1") //0: 창모드 1: 보더리스 2: 풀스크린
                {
                    //fullscreen
                    display_mode.SelectedIndex = 2;
                }
                else if (settingsReader.getValue("Display", "bBorderless") == "1")
                {
                    //borderless
                    display_mode.SelectedIndex = 1;
                }
                else
                {
                    display_mode.SelectedIndex = 0;
                }

                if (dptReader.getValue_bool("Render", "BorderlessUpscale"))
                {
                    display_useupscale.SelectedIndex = 0;
                }
                else
                {
                    display_useupscale.SelectedIndex = 1;
                }

                display_height.Text = settingsReader.getValue("Display", "iSize H");
                display_width.Text = settingsReader.getValue("Display", "iSize W");

                if (settingsReader.getValue_bool("Display", "bUseTAA")) //off fxaa taa
                {
                    graphic_aa.SelectedIndex = 2;
                }
                else if (settingsReader.getValue_bool("Display", "bFXAAEnabled"))
                {
                    graphic_aa.SelectedIndex = 1;
                }
                else
                {
                    graphic_aa.SelectedIndex = 0;
                }

                switch (System.Int32.Parse(settingsReader.getValue("Display", "iShadowMapResolution")))
                {
                    case 512:
                        graphic_shadowquality.SelectedIndex = 0;
                        break;
                    case 1024:
                        graphic_shadowquality.SelectedIndex = 1;
                        break;
                    case 2048:
                        graphic_shadowquality.SelectedIndex = 2;
                        break;

                    case 4096:
                        graphic_shadowquality.SelectedIndex = 3;
                        break;

                    case 8192:
                        graphic_shadowquality.SelectedIndex = 4; break;

                    default:
                        graphic_shadowquality.SelectedIndex = 2;
                        break;



                }

                switch ((int)System.Double.Parse(settingsReader.getValue("Display", "fShadowDistance")))
                {
                    case 2000:
                        graphic_shadowdistance.SelectedIndex = 0; break;

                    case 3000:
                        graphic_shadowdistance.SelectedIndex = 1; break;

                    case 8000:
                        graphic_shadowdistance.SelectedIndex = 2; break;

                    case 10000:
                        graphic_shadowdistance.SelectedIndex = 3; break;
                    default:
                        graphic_shadowdistance.SelectedIndex = 2;
                        break;
                }

                if (settingsReader.getValue_bool("Display", "bVolumetricLightingEnable"))
                {
                    switch (System.Int32.Parse(settingsReader.getValue("Display", "iVolumetricLightingQuality")))
                    {
                        case 0:
                            graphic_godrayquality.SelectedIndex = 1; break;
                        case 1:
                            graphic_godrayquality.SelectedIndex = 2; break;
                        case 2:
                            graphic_godrayquality.SelectedIndex = 3; break;
                        default:
                            graphic_godrayquality.SelectedIndex = 2;
                            break;
                    }
                }
                else
                {
                    graphic_godrayquality.SelectedIndex = 0;
                }

                graphic_lensflare.IsChecked = settingsReader.getValue_bool("Imagespace", "bLensFlare");
                graphic_ssao.IsChecked = settingsReader.getValue_bool("Display", "bSAOEnable");
                graphic_ssr.IsChecked = settingsReader.getValue_bool("Display", "bScreenSpaceReflectionEnabled");

                launcher_useonline.IsChecked = AppSettings.Get_useOnline();
                launcher_firststartup.IsChecked = AppSettings.Get_isFirstStartup();
                launcher_useExperimental.IsChecked = AppSettings.Get_useExperimental();
                launcher_skyrimdir.Text = AppSettings.Get_path_skyrim();
                launcher_modpackdir.Text = AppSettings.Get_path_modpack();

            }
            catch (Exception ex)
            {
                Growl.Error($"{ex}");
            }
        }

        private void btn_applyandexit(object sender, RoutedEventArgs e)
        {
            try
            {
                //디스플레이 모드
                switch (display_mode.SelectedIndex)
                {
                    case 0: //창모드
                        settingsReader.changeValue("Display", "bFull Screen", "0");
                        break;

                    case 1: //보더리스
                        settingsReader.changeValue("Display", "bFull Screen", "0");
                        settingsReader.changeValue("Display", "bBorderless", "1");
                        break;

                    case 2: //전체화면
                        settingsReader.changeValue("Display", "bFull Screen", "1");
                        break;

                }

                //업스케일 유무
                switch (display_useupscale.SelectedIndex)
                {
                    case 0: //예
                        dptReader.changeValue("Render", "BorderlessUpscale", "true");
                        break;
                    case 1: //아니오
                        dptReader.changeValue("Render", "BorderlessUpscale", "false");
                        break;

                }

                //해상도 적용
                settingsReader.changeValue("Display", "iSize H", display_height.Text);
                settingsReader.changeValue("Display", "iSize W", display_width.Text);


                //안티
                switch (graphic_aa.SelectedIndex)
                {
                    case 0: //off
                        settingsReader.changeValue("Display", "bUseTAA", "0");
                        settingsReader.changeValue("Display", "bFXAAEnabled", "0");
                        break;

                    case 1: //FXAA
                        settingsReader.changeValue("Display", "bUseTAA", "0");
                        settingsReader.changeValue("Display", "bFXAAEnabled", "1");
                        break;

                    case 2:
                        settingsReader.changeValue("Display", "bUseTAA", "1");
                        settingsReader.changeValue("Display", "bFXAAEnabled", "0");
                        break;
                }

                //그림자 품질
                switch (graphic_shadowquality.SelectedIndex)
                {
                    case 0:
                        settingsReader.changeValue("Display", "iShadowMapResolution", "512");
                        break;

                    case 1:
                        settingsReader.changeValue("Display", "iShadowMapResolution", "1024");
                        break;

                    case 2:
                        settingsReader.changeValue("Display", "iShadowMapResolution", "2048");
                        break;

                    case 3:
                        settingsReader.changeValue("Display", "iShadowMapResolution", "4096");
                        break;

                    case 4:
                        settingsReader.changeValue("Display", "iShadowMapResolution", "8192");
                        break;
                }

                //그림자 거리
                switch (graphic_shadowdistance.SelectedIndex) // 2 3 8 10
                {
                    case 0:
                        settingsReader.changeValue("Display", "fShadowDistance", "2000");
                        break;

                    case 1:
                        settingsReader.changeValue("Display", "fShadowDistance", "3000");
                        break;

                    case 2:
                        settingsReader.changeValue("Display", "fShadowDistance", "8000");
                        break;

                    case 3:
                        settingsReader.changeValue("Display", "fShadowDistance", "10000");
                        break;
                }

                //갓레이 품질
                switch (graphic_godrayquality.SelectedIndex)
                {
                    case 0: //off
                        settingsReader.changeValue("Display", "bVolumetricLightingEnable", "0");
                        settingsReader.changeValue("Display", "iVolumetricLightingQuality", "0");
                        break;

                    case 1:
                        settingsReader.changeValue("Display", "bVolumetricLightingEnable", "1");
                        settingsReader.changeValue("Display", "iVolumetricLightingQuality", "0");
                        break;

                    case 2:
                        settingsReader.changeValue("Display", "bVolumetricLightingEnable", "1");
                        settingsReader.changeValue("Display", "iVolumetricLightingQuality", "1");
                        break;

                    case 3:
                        settingsReader.changeValue("Display", "bVolumetricLightingEnable", "1");
                        settingsReader.changeValue("Display", "iVolumetricLightingQuality", "2");
                        break;
                }

                //화공반
                settingsReader.changeValue("Display", "bScreenSpaceReflectionEnabled", (Convert.ToInt32((bool)graphic_ssr.IsChecked)).ToString());
                //SSAO
                settingsReader.changeValue("Display", "bSAOEnable", (Convert.ToInt32((bool)graphic_ssao.IsChecked)).ToString());
                //렌즈
                settingsReader.changeValue("Imagespace", "bLensFlare", (Convert.ToInt32((bool)graphic_lensflare.IsChecked)).ToString());


                //모드팩 설정
                AppSettings.Set_isFirstStartup((bool)launcher_firststartup.IsChecked);
                AppSettings.Set_useOnline((bool)launcher_useonline.IsChecked);
                AppSettings.Set_useExperimental((bool)launcher_useExperimental.IsChecked);
                AppSettings.Set_path_modpack(launcher_modpackdir.Text);
                AppSettings.Set_path_skyrim(launcher_skyrimdir.Text);

                //설정 화면 닫고 복귀
                System.Windows.Window launcherwindow = new window_launcher();
                launcherwindow.Show();
                this.Close();

            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal($"오류가 발생하여 설정 적용에 실패했습니다 : {ex.Message}");
            }
        }

        private void btn_closewithoutsaving_click(object sender, RoutedEventArgs e)
        {
            //물어봅시다
            if (HandyControl.Controls.MessageBox.Show("변경 사항을 취소하고 런처 메인화면으로 돌아갑니다. 계속하시겠습니까?", null, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                //설정 화면 닫고 복귀
                System.Windows.Window launcherwindow = new window_launcher();
                launcherwindow.Show();
                this.Close();
            }

        }

        private void btn_showkeycode_click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.creationkit.com/index.php?title=Input_Script#DXScanCodes");
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
                launcher_skyrimdir.Text = folderBrowserDialog1.SelectedPath;

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
                launcher_modpackdir.Text = openFileDialog1.SelectedPath;

            }
            catch (Exception ex)
            {
                Growl.Error($"{ex.Message}");
            }
        }
        private void btn_restlauncher_click(object sender, RoutedEventArgs e)
        {
            if (HandyControl.Controls.MessageBox.Show("모든 런처 설정을 초기화하고 모드팩 초기 설정 화면으로 재가동 합니다. 계속하시겠습니까?", null, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                AppSettings.Set_useOnline(true);
                AppSettings.Set_isFirstStartup(true);
                AppSettings.Set_path_modpack("");
                AppSettings.Set_path_skyrim("");
                System.Windows.Window window_main = new MainWindow();
                window_main.Show();
                this.Close();
            }

        }
    }
}
