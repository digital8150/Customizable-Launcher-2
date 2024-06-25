using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SGLauncher2._0.Windows
{
    /// <summary>
    /// window_downgrade.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class window_downgrade : Window
    {

        private int page = 1;
        private string googledrive = "";
        private static HttpClient httpClient = new HttpClient();
        public window_downgrade()
        {
            InitializeComponent();
            page = 1;
            refreshpage();
            retriveVersions();
        }

        private async void retriveVersions()
        {

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync("http://dev.codingbot.kr/sgnetwork/fullpatcher.txt");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                googledrive = responseBody;
            }
            catch (Exception ex)
            {
                
            }


        }


        private void gotonexus(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.nexusmods.com/skyrimspecialedition/mods/57618/?tab=files");
        }

        private void gotogoogledrive(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(googledrive);
        }

        private void refreshpage()
        {
            page_1.Visibility = Visibility.Collapsed;
            page_2.Visibility = Visibility.Collapsed;
            page_3.Visibility = Visibility.Collapsed;
            button_next.Visibility = Visibility.Visible;

            switch (page)
            {
                case 1:
                    page_1.Visibility = Visibility.Visible;
                    break;

                case 2:
                    page_2.Visibility = Visibility.Visible;
                    break;

                case 3:
                    page_3.Visibility = Visibility.Visible;
                    button_next.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void next(object sender, RoutedEventArgs e)
        {
            if(page < 3)
            {
                page++;
            }

            refreshpage();

        }

        private void previous(object sender, RoutedEventArgs e)
        {
            if (page > 1)
            {
                page--;
            }

            refreshpage();
        }

        private void btn_close_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
