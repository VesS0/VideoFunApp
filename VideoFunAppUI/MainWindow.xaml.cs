using Microsoft.Win32;
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
        Transcript.Language currentTranscriptLanguage;

        public MainWindow()
        {
            InitializeComponent();
            PopulateComboBoxWithTranscriptions();
        }

        private void PopulateComboBoxWithTranscriptions()
        {
            foreach (var lang in (Language[])Enum.GetValues(typeof(Transcription.Language)))
            {
                var comboBoxItem = new ComboBoxItem() { Content = lang.ToString() };
                comboBoxItem.Selected += TranscribeTo;

                ComboTranscription.Items.Add(comboBoxItem);
            }

            ComboTranscription.Text = "Select Language of your Video";
        }

        private void PopulateComboBoxWithTranslations()
        {
            foreach (var lang in (Language[])Enum.GetValues(typeof(Language)))
            {
                var comboBoxItem = new ComboBoxItem() { Content = lang.ToString() };
                comboBoxItem.Selected += TranslateTo;

                ComboTranslation.Items.Add(comboBoxItem) ;
            }

            ComboTranslation.IsEnabled = true;
        }

        private void LoadVideoAndTranscript(string videoPath)
        {
            var video = new Video(videoPath);

            mePlayer.Source = new Uri(video.Path);

            var audio = ffmpeg.ExtractAudio(video);

            currentTranscript = new Transcript(audio, currentTranscriptLanguage);

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

        private void TranscribeTo(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)sender;

            if (selectedItem == null)
            {
                return;
            }

            if (!Enum.TryParse((string)selectedItem.Content, out currentTranscriptLanguage))
            {
                throw new Exception("Unsuported language");
            }

            VideoSelection.IsEnabled = true;
        }

        private void TranslateTo(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)sender;

            if (selectedItem == null)
            {
                return;
            }

            if (!Enum.TryParse((string)selectedItem.Content, out Language lang))
            {
                throw new Exception("Unsuported language");
            }

            var translation = new TranslateText(currentTranscript, new Language[] { lang });

            textBox.Text = translation.translations[0].Text;
            textBlock.Text = textBox.Text;
        }

        private void SelectVideoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if(openFileDialog.ShowDialog() == true)
            {
                tbvideoname.Text = openFileDialog.FileName;
                tbvideoname.IsEnabled = false;

                LoadVideoAndTranscript(openFileDialog.FileName);
                PopulateComboBoxWithTranslations();

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
            }

            btnPause.IsEnabled = true;
            btnPlay.IsEnabled = true;
            btnStop.IsEnabled = true;
        }
    }
}
