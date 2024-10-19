using Round.WPF.MusicPlayer.Module.Cs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// VideoListButton.xaml 的交互逻辑
    /// </summary>
    public partial class VideoListButton : UserControl
    {
        VideoListButtonConfig Config;
        public VideoListButton(VideoListButtonConfig videoListButtonConfig)
        {
            InitializeComponent();
            try
            {
                // 创建BitmapImage并设置解码
                BitmapImage bitmap = new BitmapImage();

                // 设置 BitmapImage 的 UriSource
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(videoListButtonConfig.ImagePath);
                bitmap.EndInit();

                // 将BitmapImage设置为Image控件的源
                ImageShow.Source = bitmap;

                bitmap.Clone();

                TitleShow.Content = videoListButtonConfig.Title;
            }
            catch { }
            Config = videoListButtonConfig;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GL.Main.MainPlayerBox.SetPlayer(Config);
        }
    }
}
