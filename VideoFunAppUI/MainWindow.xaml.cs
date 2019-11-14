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
using System.Windows.Threading;
using Transcription;

namespace VideoFunAppUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Transcript currentTranscript;

        public MainWindow()
        {
            InitializeComponent();

            LoadVideoAndTranscript(@"C:\Repos\video-1573565254.mp4");
            PopulateComboBoxWithTranslations();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private void PopulateComboBoxWithTranslations()
        {
            //foreach (var lang in Transcription.Language)
        }

        private void LoadVideoAndTranscript(string videoPath)
        {
            var video = new Video(videoPath);

            mePlayer.Source = new Uri(video.Path);

            var audio = ffmpeg.ExtractAudio(video);

            currentTranscript = new Transcript(audio, Transcript.Language.French);

            textBlock.Text = currentTranscript.TranscriptBulkText.Value;

            textBox.Text = textBlock.Text;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan)
                    lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
            }
            else
                lblStatus.Content = "No file selected...";
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
        }

        public void ChangeMediaElement(string newSource)
        {
            mePlayer.Source = new Uri(newSource);
        }

        private void changeTo(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)sender;

            if (selectedItem == null)
            {
                return;
            }

            var selectedLanguage = selectedItem.Tag;

            textBox.Text = (string)selectedLanguage;
            textBlock.Text = (string)selectedLanguage;
        }
    }
}
