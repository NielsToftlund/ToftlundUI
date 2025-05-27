using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace StatusBar
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

        private async void TrykHerKlik(object sender, RoutedEventArgs e)
        {
            // Initialisere progressbaren, så den virker fra async
            var progress = new Progress<int>(value => { ccStatus.Value = value; });
            await Task.Run(() => NogetArbejde(progress));
        }

        private static async Task NogetArbejde(IProgress<int> progressprocent)
        {
            int i = 0;
            int Højest = 100;
            while (i <= Højest)
            {
                progressprocent.Report(i);
                i++;
                await Task.Delay(100);
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            var sInfo = new System.Diagnostics.ProcessStartInfo(link.NavigateUri.AbsoluteUri)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
        }
    }
}
