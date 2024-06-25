using SGLauncher2._0.Classes;
using System.Windows;

namespace SGLauncher2._0
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //process appflow with direction
        private void window_loaded(object sender, RoutedEventArgs e)
        {
            if (AppSettings.Get_isFirstStartup())
            {
                Window installWindow = new SGLauncher2._0.Windows.window_Installation();
                installWindow.Show();
                this.Close();
            }
            else
            {
                //런처 실행
                Window launcherWindow = new SGLauncher2._0.Windows.window_launcher();
                launcherWindow.Show();
                this.Close();
            }

        }
    }
}
