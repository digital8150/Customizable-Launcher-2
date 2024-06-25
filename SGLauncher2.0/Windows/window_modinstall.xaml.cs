
using HandyControl.Controls;
using SGLauncher2._0.Classes;
using SGLauncher2._0.Properties;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace SGLauncher2._0.Windows
{
    /// <summary>
    /// window_modinstall.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class window_modinstall : System.Windows.Window
    {
        private iniFileHelper moIniFileReader;
        private string selectedProfile = null;
        private string settingFilePath = null;
        private string modFolderPath = null;
        private string loadorder = null;
        private string plugins = null;

        private void window_dragmove(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        public window_modinstall()
        {
            InitializeComponent();
            moIniFileReader = new iniFileHelper($"{AppSettings.Get_path_modpack()}ModOrganizer.ini");


        }

        private void Window_loaded(object sender, RoutedEventArgs e)
        {

            

            //오거나이저 설정 읽기
            try
            {
                //Regex.Match(moiniReader.getValue("General", "selected_profile"), @"\@ByteArray\((.*?)\)").Groups[1].Value
                //선택된 프로필
                selectedProfile = Regex.Match(moIniFileReader.getValue("General", "selected_profile"), @"\@ByteArray\((.*?)\)").Groups[1].Value;
                settingFilePath = $"{AppSettings.Get_path_modpack().Substring(0, AppSettings.Get_path_modpack().Length - 1)}{AppSettings.Get_add_base_dir()}\\profiles\\{selectedProfile}\\modlist.txt";
                modFolderPath = $"{AppSettings.Get_path_modpack().Substring(0, AppSettings.Get_path_modpack().Length - 1)}{AppSettings.Get_add_base_dir()}\\mods";
                loadorder = $"{AppSettings.Get_path_modpack().Substring(0, AppSettings.Get_path_modpack().Length - 1)}{AppSettings.Get_add_base_dir()}\\profiles\\{selectedProfile}\\loadorder.txt";
                plugins = $"{AppSettings.Get_path_modpack().Substring(0, AppSettings.Get_path_modpack().Length - 1)}{AppSettings.Get_add_base_dir()}\\profiles\\{selectedProfile}\\plugins.txt";

                text_info.Text =
                    $"활성 프로필 : {selectedProfile}\n" +
                    $"설정 파일 경로 : {settingFilePath}\n" +
                    $"모드 폴더 경로 : {modFolderPath}\n";
            }
            catch (Exception ex)
            {
                HandyControl.Controls.Growl.Error($"모드 오거나이저 설정을 읽는 중 오류 발생 : {ex}");
            }

            //세퍼레이터 생성
            try
            {
                if (!System.IO.Directory.Exists($"{AppSettings.Get_path_modpack().Substring(0, AppSettings.Get_path_modpack().Length - 1)}{AppSettings.Get_add_base_dir()}\\mods\\LAUNCHER 에서 설치_separator"))
                {
                    Directory.CreateDirectory($"{AppSettings.Get_path_modpack().Substring(0, AppSettings.Get_path_modpack().Length - 1)}{AppSettings.Get_add_base_dir()}\\mods\\LAUNCHER 에서 설치_separator");
                    // 기존 파일의 내용을 읽기
                    string[] lines = File.ReadAllLines(settingFilePath);

                    // 두 번째 줄에 삽입할 텍스트
                    string newText = "-LAUNCHER 에서 설치_separator";

                    // 기존 내용을 그대로 유지하면서 두 번째 줄에 텍스트 추가
                    lines = InsertAt(lines, 1, newText);

                    // 변경된 내용을 파일에 쓰기
                    File.WriteAllLines(settingFilePath, lines);
                }
            }
            catch (Exception ex)
            {
                HandyControl.Controls.Growl.Error($"경로 생성 중 예외 발생 {ex}");

            }
        }

        //압축인지 판별
        private bool IsZipFile(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
                    {
                        // 파일이 유효한 ZIP 아카이브인 경우 true를 반환
                        return true;
                    }
                }
            }
            catch
            {
                // 파일이 ZIP 아카이브가 아닌 경우나 오류가 발생한 경우 false를 반환
                return false;
            }
        }

        //파일드롭
        private void DropBorder_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    if (IsZipFile(file))
                    {
                        // 파일이 압축 파일인 경우

                        if (!Directory.Exists($"{modFolderPath}\\{System.IO.Path.GetFileNameWithoutExtension(file)}"))
                        {
                            DropBorder.Visibility = Visibility.Hidden;

                            //경로 생성 후
                            Directory.CreateDirectory($"{modFolderPath}\\{System.IO.Path.GetFileNameWithoutExtension(file)}");

                            //압축 해제
                            new Thread(() =>
                            {
                                unZip(file, $"{modFolderPath}\\{System.IO.Path.GetFileNameWithoutExtension(file)}");

                                string directoryPath = $"{modFolderPath}\\{System.IO.Path.GetFileNameWithoutExtension(file)}\\Data";
                                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
                                try
                                {
                                    //Data 폴더 수정

                                    if (directoryInfo.Exists)
                                    {

                                        // "Data" 폴더의 내용물을 모두 밖으로 빼낸다.
                                        foreach (DirectoryInfo childDirectory in directoryInfo.GetDirectories())
                                        {
                                            childDirectory.MoveTo($"{modFolderPath}\\{System.IO.Path.GetFileNameWithoutExtension(file)}\\{childDirectory.Name}");
                                        }
                                        foreach (FileInfo file1 in directoryInfo.GetFiles())
                                        {
                                            file1.MoveTo($"{modFolderPath}\\{System.IO.Path.GetFileNameWithoutExtension(file)}\\{file1.Name}");
                                        }

                                        // "Data" 폴더를 삭제한다.
                                        directoryInfo.Delete();



                                    }
                                }
                                catch (Exception ex)
                                {
                                    Growl.Error($"모드 구조 수정 중 예외가 발생했습니다. {ex.Message}");
                                }



                                // Read file contents
                                string[] lines = File.ReadAllLines(settingFilePath);

                                // Find the index of the line that contains "-SGAYRIM LAUNCHER 에서 설치_separator"
                                int separatorIndex = Array.IndexOf(lines, "-LAUNCHER 에서 설치_separator");

                                // Create a new array that is one element larger than the original array
                                string[] newLines = new string[lines.Length + 1];

                                // Copy the elements of the original array into the new array
                                Array.Copy(lines, newLines, separatorIndex);

                                // Insert the new line at the desired index
                                newLines[separatorIndex] = $"+{System.IO.Path.GetFileNameWithoutExtension(file)}";

                                // Copy the remaining elements of the original array into the new array
                                Array.Copy(lines, separatorIndex, newLines, separatorIndex + 1, lines.Length - separatorIndex);

                                // Assign the new array to the original array variable
                                lines = newLines;

                                // Write the modified lines to the file
                                File.WriteAllLines(settingFilePath, lines);




                                directoryPath = $"{modFolderPath}\\{System.IO.Path.GetFileNameWithoutExtension(file)}";
                                // 디렉토리 안의 파일 목록을 가져온다.
                                directoryInfo = new DirectoryInfo(directoryPath);
                                FileInfo[] fileInfos = directoryInfo.GetFiles();

                                // 확장자가 ".esp" 인 파일을 찾는다.
                                foreach (FileInfo fileInfo in fileInfos)
                                {
                                    // 확장자를 가져온다.
                                    string extension = System.IO.Path.GetExtension(fileInfo.Name);

                                    // 확장자가 ".esp" 인 경우
                                    if (extension == ".esp")
                                    {
                                        //loadorder, plugins를 업데이트
                                        using (StreamWriter sw = new StreamWriter(loadorder, true))
                                        {
                                            // Write the line "this is add line" to the bottom of the file
                                            sw.WriteLine($"{fileInfo.Name}");
                                        }

                                        using (StreamWriter sw = new StreamWriter(plugins, true))
                                        {
                                            // Write the line "this is add line" to the bottom of the file
                                            sw.WriteLine($"*{fileInfo.Name}");
                                        }

                                    }
                                }

                            }).Start();










                        }
                        else
                        {
                            Growl.Warning($"동일한 이름의 모드가 이미 설치되어 있어 진행되지 않았습니다.");
                        }

                    }
                    else
                    {
                        Growl.Warning($"지원되지 않는 형식 또는 손상된 파일입니다 : {file}");
                    }
                }
            }
        }

        public void unZip(string zipfilepath, string extractdir)
        {
            string zipPath = zipfilepath;
            string extractPath = extractdir;

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    int count = 0;
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
                            count++;
                            Dispatcher.Invoke(() => { progress_install.Value = ((double)count / (double)archive.Entries.Count) * 100; });
                        }




                    }

                    Dispatcher.Invoke(() => { DropBorder.Visibility = Visibility.Visible; });
                    Growl.Info($"모드 설치 성공! : {extractdir}");
                }

            }
            catch (Exception ex)
            {
                Growl.Error($"압축 해제 중 오류 발생 : {ex.Message}");
            }



        }

        static T[] InsertAt<T>(T[] array, int index, T item)
        {
            T[] newArray = new T[array.Length + 1];

            for (int i = 0, j = 0; i < newArray.Length; i++)
            {
                if (i == index)
                {
                    newArray[i] = item;
                }
                else
                {
                    newArray[i] = array[j];
                    j++;
                }
            }

            return newArray;
        }

        private void btn_close_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
