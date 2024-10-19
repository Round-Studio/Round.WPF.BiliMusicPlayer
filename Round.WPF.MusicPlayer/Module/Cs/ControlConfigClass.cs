using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Round.WPF.MusicPlayer.Module.Cs
{
    public class ControlConfigClass
    {
        public class VideoShowButtonConfig
        {
            public string Title { get; set; }
            public string ImageURL { get; set; }
            public string VideoURL { get; set; }
        }
        public class VideoListButtonConfig
        {
            public string Title { get; set; }
            public string ImagePath { get; set; }
            public string PlayerPath { get; set; }
        }
    }
}
