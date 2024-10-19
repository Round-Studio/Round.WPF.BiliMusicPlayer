using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static Round.WPF.MusicPlayer.Module.Cs.ControlConfigClass;

namespace Round.WPF.MusicPlayer.Module.AppControls
{
    public partial class PlayerBox : UserControl
    {
        private bool isPlaying = false;
        private DispatcherTimer timer;
        private IWaveSource waveSource;
        private ISoundOut soundOut;
        private double duration;

        public PlayerBox()
        {
            InitializeComponent();
            InitializeMediaPlayer();
        }

        private void InitializeMediaPlayer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            duration = waveSource.Length / waveSource.WaveFormat.SampleRate;
            progressSlider.Maximum = duration;
            timer.Start();
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            soundOut.Stop();
            progressSlider.Value = 0;
            isPlaying = false;
            playPauseButton.Content = "Play";
            timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isPlaying)
            {
                //double currentPosition = (double)waveSource.GetPosition();
                progressSlider.Value ++;
                currentTimeLabel.Content = waveSource.GetPosition().ToString(@"mm\:ss");
                //Console.WriteLine((long)(waveSource.GetLength().TotalSeconds - long.Parse(waveSource.GetPosition().ToString("ss"))));
                double totalSeconds = progressSlider.Maximum-progressSlider.Value;
                int minutes = (int)totalSeconds / 60;
                int seconds = (int)totalSeconds % 60;
                // 格式化为 mm:ss
                string timeString = $"{minutes:00}:{seconds:00}";
                Console.WriteLine(timeString);
                remainingTimeLabel.Content = timeString;
            }
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
            {
                soundOut.Pause();
                isPlaying = false;
                playPauseButton.Content = "Play";
            }
            else
            {
                soundOut.Play();
                isPlaying = true;
                playPauseButton.Content = "Pause";
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            /*if (soundOut != null && waveSource != null && waveSource.CanSeek)
            {
                var position = TimeSpan.FromSeconds(progressSlider.Value);
                waveSource.SetPosition(position);
                soundOut.Pause();
                soundOut.Play();
                isPlaying = true;
                playPauseButton.Content = "Pause";
            }*/
        }

        private void ProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (soundOut != null && waveSource != null && waveSource.CanSeek)
            {
                var position = TimeSpan.FromSeconds(progressSlider.Value);
                waveSource.SetPosition(position);
                soundOut.Pause();
                soundOut.Play();
                isPlaying = true;
                playPauseButton.Content = "Pause";
            }
        }

        public void SetPlayer(VideoListButtonConfig videoListButtonConfig)
        {
            soundOut?.Stop();
            timer.Start();
            waveSource?.Dispose();

            waveSource = CodecFactory.Instance.GetCodec(videoListButtonConfig.PlayerPath).ToSampleSource().ToWaveSource();
            soundOut = new WasapiOut();
            soundOut.Initialize(waveSource);
            soundOut.Play();

            progressSlider.Maximum = waveSource.GetLength().TotalSeconds;
            progressSlider.Value = 0;

            isPlaying = true;
            playPauseButton.Content = "Pause";

            TItleSHow.Content = videoListButtonConfig.Title;
        }
    }
}