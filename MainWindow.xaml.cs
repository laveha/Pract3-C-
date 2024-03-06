using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows.Threading;
using Path = System.IO.Path;

namespace AudioPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int MediaIndex = 0;
        int PlayStopIndex = 0;
        string[] files;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void FindMusic(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                MusicList.Items.Clear();

                files = Directory.GetFiles(dialog.FileName, "*.*")
                                            .Where(s => s.EndsWith(".mp3") || s.EndsWith(".mp4"))
                                            .ToArray();
                foreach (string file in files)
                {
                    MusicList.Items.Add(Path.GetFileName(file));
                }

                PlayStop.IsEnabled = true;
                SkipNext.IsEnabled = true;
                SkipPrevious.IsEnabled = true;

                PlayMusic();
            }
        }
        private void PlayMusic()
        {
            var file = files[MediaIndex];
            Player.Source = new Uri(file);
            Player.Play();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_Tick;
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (Player.Source != null)
            {
                MediaSlider.Value = Player.Position.Ticks;
            }
        }
        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            MediaSlider.Value = 0;
            MediaSlider.Maximum = Player.NaturalDuration.TimeSpan.Ticks;
        }

        private void MediaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Position = new TimeSpan(Convert.ToInt64(MediaSlider.Value));
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (MediaIndex == files.Length - 1)
            {
                MediaIndex = 0;
            }
            else
            {
                MediaIndex++;
            }
            PlayMusic();
        }

        private void PlayStop_Click(object sender, RoutedEventArgs e)
        {
            if (PlayStopIndex == 0)
            {
                Player.Pause();
                PlayStopIndex = 1;
            }
            else if (PlayStopIndex == 1)
            {
                Player.Play();
                PlayStopIndex = 0;
            }
        }

        private void SkipPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (MediaIndex != 0)
            {
                MediaIndex -= 1;
            }
            PlayMusic();
        }

        private void SkipNext_Click(object sender, RoutedEventArgs e)
        {
            if (MediaIndex != files.Length - 1)
            {
                MediaIndex += 1;
            }
            PlayMusic();
        }
    }
}