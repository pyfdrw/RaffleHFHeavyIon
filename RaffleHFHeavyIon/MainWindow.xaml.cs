using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Uri = System.Uri;

namespace RaffleHFHeavyIon
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 抽奖使用的类
        private RaffleHelper raffle;

        private BackgroundWorker backgroundWorker;

        // 抽奖的背景音乐
        private MediaPlayer progressPlayer = new MediaPlayer();
        // 抽奖结束的背景音乐
        private MediaPlayer finalPlayer = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();

            backgroundWorker = (BackgroundWorker) this.FindResource("BackgroundWorker");

            raffle = new RaffleHelper(10, 14);
            this.DataContext = raffle;

            progressPlayer.Open(new Uri("欢乐抽奖.mp3", UriKind.Relative));
            finalPlayer.Open(new Uri("guanzhonghuhan.mp3", UriKind.Relative));
        }

        private void BackgroundWorker_OnDoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                raffle.GenerateRandomSequentially();
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
            }
            
        }

        private void BackgroundWorker_OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                raffle.AddHistory();
            }
        }

        // 为false，说明是开始
        // 为true，说明是暂停
        private bool workStatus = false;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // 点击的是开始
            if (!workStatus)
            {
                Button.Content = "暂停";

                progressPlayer.MediaEnded -= new EventHandler(Media_Ended);
                progressPlayer.MediaEnded += new EventHandler(Media_Ended);
                progressPlayer.Play();
                finalPlayer.Stop();

                backgroundWorker.RunWorkerAsync();

                workStatus = true;
            }
            else
            {
                Button.Content = "开始";

                finalPlayer.Play();

                backgroundWorker.CancelAsync();

                workStatus = false;
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            progressPlayer.Close();
            finalPlayer.Close();
        }

        // 循环播放
        private void Media_Ended(object sender, EventArgs e)
        {
            progressPlayer.Position = TimeSpan.Zero;
            progressPlayer.Play();
        }
    }
}
