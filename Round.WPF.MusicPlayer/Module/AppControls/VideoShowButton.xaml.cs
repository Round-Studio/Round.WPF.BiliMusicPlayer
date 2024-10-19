using ControlzEx.Standard;
using MahApps.Metro.Controls.Dialogs;
using Round.WPF.MusicPlayer.Module.Cs;
using Round.WPF.MusicPlayer.Module.Cs.LinkBilibiliServer;
using Round_Log_System;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Round.WPF.MusicPlayer.Module.Cs.ControlConfigClass;

namespace Round.WPF.MusicPlayer.Module.AppControls
{
    /// <summary>
    /// VideoShowButton.xaml 的交互逻辑
    /// </summary>
    public partial class VideoShowButton : UserControl
    {
        string URL;
        public VideoShowButton(VideoShowButtonConfig videoShowButtonConfig)
        {
            InitializeComponent();
            Opacity = 0.00;
            Task.Run(() =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        Opacity = i*0.01;
                    });
                    Thread.Sleep(2);
                }
            });
            TitleShow.Content = videoShowButtonConfig.Title;
            URL = "https:"+videoShowButtonConfig.VideoURL;
            Task.Run(() => {
                try
                {
                    DownloadAndDisplayImage(videoShowButtonConfig.ImageURL);
                }
                catch
                {
                    GL.Main.MainShowWrapPanel.Children.Remove(this);
                }
            });
        }
        private void DownloadAndDisplayImage(string imageUrl)
        {
            Task.Run(async () =>
            {
                // 设置HttpClient的基础地址
                using (HttpClient client = new HttpClient())
                {
                    // 如果提供的URL是相对路径，则需要设置BaseAddress
                    client.BaseAddress = new Uri("https://i2.hdslb.com"); // 请替换为实际的基础地址

                    // 获取图片数据
                    try
                    {
                        Stream stream = await client.GetStreamAsync(imageUrl.Replace("https://i2.hdslb.com", "")); // imageUrl应该是相对于BaseAddress的路径

                        ImageShow.Dispatcher.Invoke(new Action(() =>
                        {
                            // 创建BitmapImage并设置解码
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            bitmap.StreamSource = stream; // 直接使用stream，不需要使用MemoryStream
                            bitmap.EndInit();

                            // 将BitmapImage设置为Image控件的源
                            ImageShow.Source = bitmap;

                            bitmap.Clone();
                        }));
                    }
                    catch
                    {
                        GL.Main.MainShowWrapPanel.Dispatcher.Invoke(() => {
                            GL.Main.MainShowWrapPanel.Children.Remove(this);
                        });
                    }

                    Loading.Dispatcher.Invoke(() =>
                    {
                        Loading.Visibility = Visibility.Hidden;
                    });
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string Title = (string?)TitleShow.Content;
            Task.Run(() =>
            {
                string[] output = RunExternalProgramAndCaptureOutput("bin\\BBDown.exe", URL + " --audio-only --work-dir bin\\download");

                if (output[output.Length - 2].Contains("任务完成"))
                {
                    //MessageBox.Show("任务完成");
                    foreach (string outputItem in output)
                    {
                        if (outputItem.Contains("获取aid结束: "))
                        {
                            string aid = outputItem.Split("获取aid结束: ")[1];
                            RLog.Out.WriteLine("Find Video AID:" + aid);

                            File.WriteAllText("bin\\download\\" + aid + "\\" + aid + ".txt", Title);
                            foreach (var fileitem in Directory.GetFiles($"bin\\download\\{aid}"))
                            {
                                if (fileitem.Contains(".m4a"))
                                {
                                    File.WriteAllBytes("bin\\download\\" + aid + "\\" + aid + ".m4a", File.ReadAllBytes(fileitem));
                                    File.Delete(fileitem);
                                    //RunExternalProgramAndCaptureOutput("bin\\ffmpeg.exe", "-i "+ "bin\\download\\" + aid + "\\" + aid + ".m4a"+ " bin\\download\\" + aid + "\\" + aid + ".mp4");

                                    GL.Main.MainWindow.Dispatcher.Invoke(new Action(() =>
                                    {
                                        GL.Main.MainWindow.ShowMessageAsync("下载成功", Title + "\n已下载完成");
                                    }));
                                }
                            }
                        }
                    }
                }
            });
        }

        public static string[] RunExternalProgramAndCaptureOutput(string fileName, string arguments)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            // 使用 StreamReader 逐行读取输出
            using (System.IO.StreamReader reader = process.StandardOutput)
            {
                string output = reader.ReadToEnd();
                process.WaitForExit();

                string[] outputLines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                return outputLines;
            }
        }
    }
}
