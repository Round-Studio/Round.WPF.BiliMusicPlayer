using Newtonsoft.Json;
using Round.WPF.MusicPlayer.Module.Cs.LinkBilibiliServer;
using Round.WPF.MusicPlayer.Module.Cs;
using System;
using System.Collections.Generic;
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
using static Round.WPF.MusicPlayer.Module.Cs.LinkBilibiliServer.Server;
using MahApps.Metro.Controls.Dialogs;
using static Round_Log_System.RLog;

namespace Round.WPF.MusicPlayer.Module.AppControls
{
    /// <summary>
    /// SearchBox.xaml 的交互逻辑
    /// </summary>
    public partial class SearchBox : UserControl
    {
        public SearchBox()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Server.Search(TextBoxInput.Text);
        }
    }
}
