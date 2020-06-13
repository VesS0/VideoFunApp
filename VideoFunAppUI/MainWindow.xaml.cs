using Azure.AI.TextAnalytics;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Transcription;
using static Transcription.Transcript;

namespace VideoFunAppUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Transcript currentTranscript;
        Transcript.Language currentTranscriptLanguage;
        TextStartDuration textStartDuration;
        int currentIndex = -1;

        uint TextDelayInMilliseconds = 500;


        public MainWindow()
        {
            InitializeComponent();
            PopulateComboBoxWithTranscriptions();
        }

        private void PopulateComboBoxWithTranscriptions()
        {
            foreach (var lang in (Transcript.Language[])Enum.GetValues(typeof(Transcript.Language)))
            {
                var comboBoxItem = new ComboBoxItem() { Content = lang.ToString() };
                comboBoxItem.Selected += TranscribeTo;
                if(lang == Transcript.Language.EnglishUK)
                {
                    comboBoxItem.IsSelected = true;
                }
                ComboTranscription.Items.Add(comboBoxItem);
            }

            ComboTranscription.SelectedIndex = 10;
        }

        private void PopulateComboBoxWithTranslations()
        {
            foreach (var lang in (Translation.Language[])Enum.GetValues(typeof(Translation.Language)))
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
            textBlock_Analytics.Text = new TextAnalytics().EntityLinkingExample(currentTranscript.TranscriptBulkText.Value);

            textStartDuration = currentTranscript.textStartDuration;
            currentIndex = 0;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan)
                {
                    lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));

                    if (currentIndex != -1 && textStartDuration.text.Count > currentIndex)
                    {
                        if (mePlayer.Position.TotalMilliseconds > textStartDuration.start[currentIndex] + textStartDuration.duration[currentIndex] + TextDelayInMilliseconds)
                        {
                            ++currentIndex;
                        } 
                        else
                        {
                            return;
                        }
                       
                        if (currentIndex >= textStartDuration.text.Count)
                        {
                            return;
                        }

                        textBox.Text = textStartDuration.text[currentIndex];
                    }
                    
                }
            }
            else
            {
                lblStatus.Content = "No file selected...";
            }
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
            currentIndex = 0;
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

            if (!Enum.TryParse((string)selectedItem.Content, out Translation.Language lang))
            {
                throw new Exception("Unsuported language");
            }

            var translation = new Translation(currentTranscript, new Translation.Language[] { lang });

            textBlock_Translate.Text = translation.translations[0].Text;
            textStartDuration.text = translation.GetTranslatedLinesForLanguageIdx(0);
            
            //textBlock.Text = "";
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

            mePlayer.Play();
            btnPause.IsEnabled = true;
            btnPlay.IsEnabled = true;
            btnStop.IsEnabled = true;
        }
    }
}
