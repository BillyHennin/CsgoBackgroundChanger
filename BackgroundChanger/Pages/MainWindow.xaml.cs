using System;
using System.Threading;
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
            MyRegedit.CheckRegedit();
            FillList();
        }

        public async void FillList()
        {
            //TODO : Afficher ProgressAsync
            MyDirectory.CheckMyDocDir();
            const string strMessage = "Loading webm.... ";
            var controller = await this.ShowProgressAsync("Please wait", strMessage);
            WebmList.Items.Clear();
            var allWebm = MyDirectory.GetAllWebm();
            if (allWebm.Length == 0)
            {
                await controller.CloseAsync();
                await this.ShowMessageAsync("Error !", "Your webm folder is empty or does not contains any webm file");
                return;
            }

            var progress = 100 / allWebm.Length / 100;
            var i = 0.0;
            var y = 0;
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
                controller.SetProgress(i += progress);
                controller.SetMessage(strMessage + y++ + "/" + allWebm.Length);
            }
            await controller.CloseAsync();
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
                var uri = WebmPlayer.Source.ToString();
                LbTitle.Content = MyDirectory.RemoveUri(uri);
                //TODO : Get file info
                //LbInfos.Content = (new System.IO.FileInfo(uri.Replace("file:///", string.Empty))).Length;
                SetVisibility(true);
            }
        }

        private async void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            var isValid = await MyDirectory.CheckMyCsgoDir(this);
            if (!isValid.Value)
            {
                return;
            }

            MyDirectory.UpdateBackground(MyDirectory.RemoveUri(WebmPlayer.Source.ToString()));
            //TODO : Update CSGO files
            await this.ShowMessageAsync("Background changed", "Enjoy your new background !");
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e) => FillList();

        private void SetVisibility(bool isVisible)
            => WebmPlayer.Visibility = LbTitle.Visibility = LbInfos.Visibility =
                SelectBtn.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
    }
}