﻿using System;
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

namespace VideoFunAppUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            textBlock.Text = "Ajajaj puertoriko newline \r\n new line Ajajaj puertorikoAjajaj puertoriko" +
                "\r\n new line Ajajaj puertorikoAjajaj puertoriko" +
                "\r\n new line Ajajaj puertorikoAjajaj puertoriko" +
                "\r\n new line Ajajaj puertorikoAjajaj puertoriko" +
                "v" +
                "\r\n new line Ajajaj puertorikoAjajaj puertoriko" +
                "\r\n new line Ajajaj puertorikoAjajaj puertoriko" +
                "\r\n new line Ajajaj puertorikoAjajaj puertoriko";

            textBox.Text = "aPSAl pals okaso kwep lfwpl \r\n dpslokods okds okdsod ";
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
