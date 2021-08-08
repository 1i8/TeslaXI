using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using TheLeftExit.Growtopia;
using TheLeftExit.Memory;
using System.Threading;

namespace TheLeftExit.TeslaXI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Game game;
        private Process gt;

        private CancellationTokenSource src;

        private bool active;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (active)
            {
                src.Cancel();
                active = false;
                GameDataLabel.Content = "Detached";
                return;
            }

            active = true;

            var plist = Process.GetProcessesByName("Growtopia");
            if(plist.Length != 1)
            {
                MessageBox.Show("Please have exactly one instance of the game open.", "NotImplementedException");
                return;
            }

            game = new(plist.Single());
            game.UpdateAddresses(0xA04130);
            gt = plist.Single();

            src = new();

            new Task(() =>
            {
                while (!src.IsCancellationRequested)
                {
                    StringBuilder sb = new();

                    Single posx = gt.Handle.ReadSingle(game[GameValue.PlayerX]);
                    sb.AppendLine($"Player X: {posx}");

                    Dispatcher.Invoke(() => GameDataLabel.Content = sb.ToString());

                    Thread.Sleep(10);
                }
                src.Dispose();
            }).Start();
        }
    }
}
