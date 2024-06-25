using HandyControl.Controls;
using SGLauncher2._0.Classes;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SGLauncher2._0.Windows
{
    /// <summary>
    /// window_launcher.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class window_launcher : System.Windows.Window
    {
        private bool isVideoEnabled = true;

        public window_launcher()
        {
            InitializeComponent();
            border_main.Height = 0;

            if (!AppSettings.getValue_bool("Settings", "b_useExperimental", "True")) button_addoninstall.Visibility = Visibility.Collapsed;
            if(!AppSettings.getValue_bool("Settings", "b_useShortcut", "False")) button_channel.Visibility = Visibility.Collapsed;

            if (!File.Exists("@changelog.txt")) border_changelog.Visibility = Visibility.Collapsed;
            else textblock_changelog.Text = File.ReadAllText("@changelog.txt");
            title_launcher.Text = AppSettings.Get_title_launcher();
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


        //Window Features
        //Dragmove feature
        private void window_dragmove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        //Window open transition & update check
        private void Window_loaded(object sender, RoutedEventArgs e)
        {
            //open transition
            Thread animationThread = new Thread(() =>
            {

                Thread.Sleep(200);
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


            //updatecheck
            if (AppSettings.Get_useOnline())
            {
                Thread thread = new Thread(() =>
                {
                    AppUpdate.retriveVersions();
                    while(AppUpdate.isUptodate == null)
                    {
                        Thread.Sleep(200);
                    }


                    if (AppUpdate.isUptodate == false)
                    {
                        Dispatcher.Invoke(() => { badge_update.ShowBadge = true; });
                        Growl.Ask("업데이트가 있습니다. 업데이트 센터를 확인하시겠습니까?", isConfirmed =>
                        {
                            if (isConfirmed)
                            {
                                System.Windows.Window update_window = new window_update();
                                update_window.Show();
                            }

                            return true;
                        });
                    }
                });

                thread.Start();
            }

            //MoRunCheckThread
            Thread moRunCheckThread = new Thread(() =>
            {
                while (true)
                {
                    int sleepms = 150;
                    if ((Process.GetProcessesByName("ModOrganizer").Length == 0) && (Process.GetProcessesByName("SkyrimSE").Length == 0))
                    {
                        //실행중 아님
                        Dispatcher.Invoke(() =>
                        {
                            btn_modorganizer.IsEnabled = true;
                            btn_gamestart.IsEnabled = true;
                            textblock_gameisrunning.Visibility = Visibility.Collapsed;
                            sleepms = 150;
                        });
                    }
                    else
                    {
                        //실행중
                        Dispatcher.Invoke(() =>
                        {
                            btn_modorganizer.IsEnabled = false;
                            btn_gamestart.IsEnabled = false;
                            textblock_gameisrunning.Visibility = Visibility.Visible;
                            sleepms = 5000;
                        });
                    }

                    Thread.Sleep(sleepms);
                }
            }); moRunCheckThread.IsBackground = true; moRunCheckThread.Start();

        }
        //close button
        private void btn_close_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        //MediaElement Auto Repeat
        private void repeat_media(object sender, RoutedEventArgs e)
        {
            media_background.Position = TimeSpan.FromSeconds(0);
            media_background.Play();
        }


        //Footer Button Features
        private void btn_options_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window option_window = new window_options();
            option_window.Show();
            this.Close();
        }

        private void btn_troubleshoot_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window option_window = new window_troubleshoot();
            option_window.Show();
            this.Close();
        }

        private void btn_channel_click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://arca.live/b/tullius");
        }

        private void btn_update_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window update_window = new window_update();
            update_window.Show();

        }

        private void btn_screenshot_click(object sender, RoutedEventArgs e)
        {
            string startaddress = AppSettings.Get_path_screenshot();
            startaddress = startaddress.Replace("%modpackdir%", $"{AppSettings.Get_path_modpack().Substring(0, AppSettings.Get_path_modpack().Length - 1)}");
            startaddress = startaddress.Replace("%add_base_dir%", $"{AppSettings.Get_add_base_dir()}");
            try
            {
                //대치어
                //%modpackdir%
                //%add_base_dir%
                //%modpackdir%%add_base_dir%\overwrite\root

                
                Process.Start(startaddress);
            }
            catch (Exception ex)
            {
                Growl.Warning($"스크린샷 폴더를 열지 못했습니다. \n{startaddress}");
            }

        }

        private void btn_gamestart_click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process startInfo = new Process();
                startInfo.StartInfo.FileName = $"{AppSettings.Get_path_modpack()}ModOrganizer.exe";
                startInfo.StartInfo.Arguments = "moshortcut://:SKSE";
                startInfo.Start();

            }
            catch (Exception ex)
            {
                Growl.Error($"예외가 발생하여 게임 실행에 실패했습니다.\n{ex}");
            }

        }

        private void btn_modorganizer_click(object sender, RoutedEventArgs e)
        {

            try
            {
                Process startInfo = new Process();
                startInfo.StartInfo.FileName = $"{AppSettings.Get_path_modpack()}ModOrganizer.exe";
                startInfo.Start();
            }
            catch (Exception ex)
            {
                Growl.Error($"예외가 발생하여 게임 실행에 실패했습니다.\n{ex}");
            }
        }

        private void btn_togglevideo_click(object sender, RoutedEventArgs e)
        {
            if (isVideoEnabled)
            {
                isVideoEnabled = false;
                icon_togglevideo.Icon = FontAwesome6.EFontAwesomeIcon.Solid_VideoSlash;
                AnimationManager.doAnimation(media_background, OpacityProperty, 1, 0);
            }
            else
            {
                isVideoEnabled = true;
                icon_togglevideo.Icon = FontAwesome6.EFontAwesomeIcon.Solid_Video;
                AnimationManager.doAnimation(media_background, OpacityProperty, 0, 1);
            }
        }

        private void btn_addoninstall_click(object sender, RoutedEventArgs e)
        {
            System.Windows.Window addonWindow = new window_modinstall();
            addonWindow.Show();
        }
    }

    public class AnimationManager
    {
        public static void doAnimation(UIElement targetObject, DependencyProperty targetProperty, int from, int to, double duration = 0.25)
        {

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = from;
            animation.To = to;
            animation.Duration = TimeSpan.FromSeconds(duration);
            animation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
            targetObject.BeginAnimation(targetProperty, animation);

        }
    }


}
