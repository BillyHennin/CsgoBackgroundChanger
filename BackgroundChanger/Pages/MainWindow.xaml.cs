using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BackgroundChanger.Classes;
using MahApps.Metro.Controls.Dialogs;

namespace BackgroundChanger.Pages
{
    /// <inheritdoc cref="" />
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitWindow();
        }

        public async void InitWindow()
        {
            MyRegedit.CheckRegedit();
            if(await MyUpdate.CheckUpdate(this) == null) await FillList();
        }

        private void SetVisibility(bool isVisible)
            => WebmPlayer.Visibility = LbTitle.Visibility = LbInfos.Visibility =
                SelectBtn.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

        public async Task FillList()
        {
            const string msg = "Loading webm.... ";
            var controller = await this.ShowProgressAsync("Please wait", msg);
            controller.SetIndeterminate();
            WebmList.Items.Clear();
            var allWebm = MyFolders.GetAllWebm();
            if (allWebm.Length == 0)
            {
                await controller.CloseAsync();
                await this.ShowMessageAsync("Error !", "Your webm folder is empty or does not contains any webm file");
                return;
            }

            controller.Minimum = 0;
            controller.Maximum = allWebm.Length;

            var progress = 100 / allWebm.Length / 100;
            var y = 1;
            foreach (var file in allWebm)
            {
                WebmList.Items.Add(new MediaElement
                {
                    Source = new Uri(file),
                    Height = 200,
                    Width = 315,
                    Stretch = Stretch.Fill,
                    Margin = new Thickness(0, 5, 0, 5)
                });
                controller.SetProgress(y += progress);
                controller.SetMessage($"{msg}{y++}/{allWebm.Length}");
                await Task.Delay(500);
            }
            await controller.CloseAsync().ConfigureAwait(false);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WebmList.SelectedItem == null)
            {
                SetVisibility(false);
            }
            else
            {
                WebmPlayer.Source = ((MediaElement) WebmList.SelectedItem).Source;
                SetVisibility(true);
            }
        }
        private void WebmPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            LbTitle.Content = MyFolders.RemoveUri(WebmPlayer.Source.ToString());
            LbInfos.Content = WebmPlayer.NaturalVideoWidth + " x " + WebmPlayer.NaturalVideoHeight
                              + " (" + WebmPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss") + ")";
        }
        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            MyFolders.UpdateBackground(this, MyFolders.RemoveUri(WebmPlayer.Source.ToString()));
            this.ShowMessageAsync("Background changed", "Enjoy your new background !");
        }
        private async void BtnRefresh_Click(object sender, RoutedEventArgs e) => await FillList();

        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}