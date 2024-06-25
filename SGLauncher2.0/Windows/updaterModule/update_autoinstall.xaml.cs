using SGLauncher2._0.Classes;
using System;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace SGLauncher2._0.Windows.updaterModule
{
    /// <summary>
    /// update_autoinstall.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class update_autoinstall : Window
    {

        private WebClient downloader = new WebClient();
        private const string _BASEURL = "http://dev.codingbot.kr/sg30";
        private string downloadurl = "";

        public update_autoinstall()
        {
            InitializeComponent();
            downloadurl = $"{_BASEURL}/{AppUpdate.modpack_version_latest}.zip";
        }

        private void Window_loaded(object sender, RoutedEventArgs e)
        {


            Thread thread = new Thread(update);
            thread.IsBackground = true;
            thread.Start();
        }

        private void update()
        {
            try
            {
                //업데이트의 순서
                //파일 다운로드
                //파일 압축해제
                Dispatcher.Invoke(() =>
                {
                    install_task.Text = "서버로 부터 파일을 다운로드 합니다.";
                });

                Thread.Sleep(300);
                downloader.DownloadProgressChanged += UpdateProgress;
                downloader.DownloadFileCompleted += DownloadCompleted;
                downloader.DownloadFileAsync(new Uri($"{downloadurl}"), $"{AppUpdate.modpack_version_latest}.zip");
            }
            catch (Exception ex)
            {
                HandyControl.Controls.MessageBox.Show($"모드팩 업데이트 중 예외가 발생하여 중단합니다.\n{ex}", null, MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        private void UpdateProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                install_task.Text = $"서버로 부터 파일 다운로드 : {e.ProgressPercentage}%";
                install_progress.Value = e.ProgressPercentage;
            });
        }

        private void DownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Thread.Sleep(300);
            int progress = 0;

            new Thread(() => { fileHelper.unZip($"{AppUpdate.modpack_version_latest}.zip", AppSettings.Get_path_modpack(), ref progress); }).Start();
            while (progress < 100)
            {
                Dispatcher.Invoke(() =>
                {
                    install_task.Text = $"압축 해제 중 : {progress}%";
                    install_progress.Value = progress;
                });
            }

            AppUpdate.updateModpackVersionCurrent();

            Dispatcher.Invoke(() =>
            {
                status_icon.Spin = false;
                status_icon.Icon = FontAwesome6.EFontAwesomeIcon.Solid_CircleCheck;
                status_icon.Foreground = new SolidColorBrush(Colors.Green);
                status_text.Text = "업데이트 완료!";
                install_task.Text = "업데이트 완료! 5초후 자동으로 종료됩니다.";

            });
            Thread.Sleep(5000);
            Dispatcher.Invoke(() => this.Close());


        }
    }
}
