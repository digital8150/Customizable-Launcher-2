using HandyControl.Controls;
using SGLauncher2._0.Classes;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace SGLauncher2._0.Windows
{
    /// <summary>
    /// window_update.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class window_update : System.Windows.Window
    {
        public window_update()
        {
            InitializeComponent();
            border_main.Height = 0;
            textblock_modpack.Text = AppSettings.getValue("Custom", "title_modpack");
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


            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = 550;
            animation.Duration = TimeSpan.FromSeconds(0.25);
            animation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

            if (!AppSettings.Get_useOnline())
            {
                if (HandyControl.Controls.MessageBox.Show("온라인 기능이 활성화 되어 있지 않습니다. 업데이트 센터를 이용하려면 온라인 기능을 활성화 해야 합니다. 지금 활성화 하시겠습니까?", null, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    AppSettings.Set_useOnline(true);
                    AppUpdate.retriveVersions();

                }
                else
                {
                    this.Close();
                    return;
                }

            }

            border_main.BeginAnimation(HeightProperty, animation);

            while (AppUpdate.isUptodate == null)
            {
                Thread thread = new Thread(() => { AppUpdate.retriveVersions(); Thread.Sleep(150); });
                thread.Start();
                thread.Join();
                
                text_modpack_current.Text = $"설치된 버전 : {AppUpdate.modpack_version_current}";
                text_modpack_latest.Text = $"최신 버전 : {AppUpdate.modpack_version_latest}";
            }
            text_modpack_current.Text = $"설치된 버전 : {AppUpdate.modpack_version_current}";
            text_modpack_latest.Text = $"최신 버전 : {AppUpdate.modpack_version_latest}";

            if (!(bool)AppUpdate.isUptodate)
            {
                modpack_blocker.Visibility = Visibility.Hidden;
            }

        }

        private void btn_close_click(object sender, RoutedEventArgs e)
        {
            Thread Animation = new Thread(() =>
            {



                Dispatcher.Invoke(() =>
                {
                    DoubleAnimation animation = new DoubleAnimation();

                    animation.From = 550;
                    animation.To = 0;
                    animation.Duration = TimeSpan.FromSeconds(0.25);
                    animation.EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };
                    border_main.BeginAnimation(HeightProperty, animation);

                });
                Thread.Sleep(250);
                Dispatcher.Invoke(() => { this.Close(); });
            });

            Animation.Start();
        }

        private void btn_modpack_autoupdate(object sender, RoutedEventArgs e)
        {
            Process.Start(AppUpdate.update_url);
        }
    }
}
