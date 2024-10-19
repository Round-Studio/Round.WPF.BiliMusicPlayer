using MahApps.Metro.Controls;
using Newtonsoft.Json;
using Round.WPF.MusicPlayer.Module.AppControls;
using Round.WPF.MusicPlayer.Module.Cs;
using Round.WPF.MusicPlayer.Module.Cs.LinkBilibiliServer;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Round.WPF.MusicPlayer.Module.Cs.LinkBilibiliServer.Server;

namespace Round.WPF.MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            Directory.CreateDirectory("bin");
            Directory.CreateDirectory("bin\\download");
            try
            {
                File.WriteAllBytes("bin\\ffmpeg.exe", global::Round.WPF.MusicPlayer.Properties.Resources.ffmpeg);
                File.WriteAllBytes("bin\\BBDown.exe", global::Round.WPF.MusicPlayer.Properties.Resources.BBDown);
            }
            catch (Exception ex) { }

            InitializeComponent();
            GL.Main.MainShowWrapPanel = WrapBox;
            GL.Main.MainPlayerList = PlayerList;
            GL.Main.TabControl = Tabs;
            GL.Main.MainPlayerBox = MainPlayerBox;

            //Server.Search("Minecraft");
            GL.Main.MainShowWrapPanel.Children.Add(new Label
            {
                Content = "\n\n在上方搜索框键入关键词以进行搜索",
                FontSize = 24,
                Foreground = new SolidColorBrush(new Color
                {
                    R = 124,
                    G = 124,
                    B = 124,
                    A = 100
                }),
            });

            Task.Run(() => {
                while (true) {
                    this.Dispatcher.Invoke(new Action(() => {
                        GL.Main.MainPlayerList.Children.Clear();
                        string[] files = Directory.GetFiles("bin\\download", "*", SearchOption.AllDirectories);

                        // 输出文件路径
                        foreach (string file in files)
                        {
                            if (file.Contains(".jpg"))
                            { 
                                if(File.Exists(file.Replace(".jpg", ".txt")))
                                {
                                    GL.Main.MainPlayerList.Children.Add(new VideoListButton(new ControlConfigClass.VideoListButtonConfig
                                    {
                                        Title = File.ReadAllText(file.Replace(".jpg", ".txt")),
                                        ImagePath = System.IO.Path.GetFullPath(file),
                                        PlayerPath = System.IO.Path.GetFullPath(file.Replace(".jpg", ".m4a"))
                                    }));
                                }
                            }
                        }
                    }));
                    Thread.Sleep(1000);
                }
            });

            GL.Main.MainWindow = this;
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MinWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}