using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using BackgroundChanger.Classes;
using MahApps.Metro.Controls.Dialogs;

namespace BackgroundChanger.Pages
{
    /// <summary>
    ///     Logique d'interaction pour MainWindow.xaml
    /// </summary>
    // ReSharper disable once InheritdocConsiderUsage
    public partial class MainWindow
    {
        private const string DefError = "Error !";

        public MainWindow()
        {
            InitializeComponent();
            //Needs to be on a separate func cause async
            InitWindow();
        }

        public async void InitWindow()
        {
            await MonitorProcess.InitMonitoringAsync(this);
            //Check if regedit is set up
            Regedit.CheckRegedit();
            //Wait for the app to check for update
            if (await Update.CheckUpdate(this) == null)
            {
                //Once it's done, fill the listview with videos
                await FillList();
            }
            
        }

        /// <summary>
        /// Updates visibility param for some element in the page
        /// </summary>
        /// <param name="visible">True or False</param>
        private void SetVisibility(bool visible) =>
                WebmPlayer.Visibility = LbTitle.Visibility = LbInfos.Visibility =
                        BtnSelect.Visibility = visible ? Visibility.Visible : Visibility.Hidden;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Void</returns>
        public async Task FillList()
        {
            var allWebm = Folders.GetAllWebm();
            if (allWebm.Length == 0)
            {
                allWebm = await UpdateFolder();
            }

            const string msg = "Loading webm.... ";
            //Init the progress dialog
            var controller = await this.ShowProgressAsync("Please wait", msg);
            controller.SetIndeterminate();
            WebmList.Items.Clear();
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
                //Let the user know how the progress is going
                controller.SetProgress(y += progress);
                controller.SetMessage($"{msg} {y++}/{allWebm.Length}");
                //Ikr, but the progress is lit af
                await Task.Delay(800 / allWebm.Length);
            }
            //Once it's down, close the controller
            await controller.CloseAsync().ConfigureAwait(false);
        }

        private async Task<string[]> UpdateFolder(bool firstTime = true)
        {
            if (firstTime)
            {
                var result = await this.ShowMessageAsync(DefError,
                        "Your webm folder is empty or does not contains any webm file, do you want to update it ?",
                        MessageDialogStyle.AffirmativeAndNegative);
                if (result == MessageDialogResult.Affirmative)
                {
                    Folders.UpdateWebmFolder();
                }
            }
            else
            {
                Folders.UpdateWebmFolder();
            }

            var allWebm = Folders.GetAllWebm();
            // ReSharper disable once InvertIf
            if (allWebm.Length == 0)
            {
                var result2 = await this.ShowMessageAsync(DefError,
                        "This app cannot work without a valid webm folder, do you want to update yours ?",
                        MessageDialogStyle.AffirmativeAndNegative);
                if (result2 == MessageDialogResult.Affirmative)
                {
                    // ReSharper disable once TailRecursiveCall
                    return await UpdateFolder(false);
                }
                Environment.Exit(1);
            }
            return allWebm;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WebmList.SelectedItem == null)
            {
                InfoBtn.Visibility = Visibility.Hidden;
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
            InfoBtn.Visibility = Visibility.Visible;
            LbTitle.Content = Folders.GetFileName(WebmPlayer.Source.AbsolutePath);
            if (!WebmPlayer.NaturalDuration.HasTimeSpan) { return; }
            var duration = string.Empty;
            try
            {
                duration = $"{WebmPlayer.NaturalDuration.TimeSpan:mm\\:ss}";
            }
            finally
            {
                LbInfos.Content = $"{WebmPlayer.NaturalVideoWidth} x {WebmPlayer.NaturalVideoHeight} ({duration})";
            }
        }

        private async void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if(MonitorProcess.IsCSOpen())
            {
                await this.ShowMessageAsync(DefError, "Please close your game.");
                return;
            }
            if (await Folders.UpdateBackground(this, WebmPlayer.Source.AbsolutePath.Replace("%20", " ")))
            {
                await this.ShowMessageAsync("Background changed", "Enjoy your new background !");
            }
            else
            {
                await this.ShowMessageAsync(DefError, "This application cannot work without a specified Counter-Strike Global Offensive folder.");
            }
        }

        private async void BtnRefresh_Click(object sender, RoutedEventArgs e) => await FillList();

        private async void BtnChangeWebmFolder_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowMessageAsync("Change your folder", "Do you want to change your webm folder", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                Folders.UpdateWebmFolder();
            }
        }

        private async void BtnChangeCSGOFolder_Click(object sender, RoutedEventArgs e)
        {
            if (await (this).ShowMessageAsync("Change your folder", "Do you want to change your Counter-Strike Global Offensive folder", MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Affirmative)
            {
                return;
            }

            using (var fbd = new FolderBrowserDialog { Description = @"Select your Counter-Strike Global Offensive folder." })
            {
                var resultFolder = fbd.ShowDialog();
                if (resultFolder != System.Windows.Forms.DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return;
                }

                if (Folders.IsCsgoFolderValid(fbd.SelectedPath))
                {
                    Regedit.MyCsgoFolderPath = fbd.SelectedPath;
                }
                else
                {
                    await this.ShowMessageAsync(DefError, "This application cannot work without a valid Counter-Strike Global Offensive folder.");
                }
            }
        }

        private async void BtnCheckUpdate_Click(object sender, RoutedEventArgs e) => await Update.CheckUpdate(this);

        private void InfoBtn_Click(object sender, RoutedEventArgs e)
        {
            var text  = $"Video size : {WebmPlayer.NaturalVideoWidth} x {WebmPlayer.NaturalVideoHeight}\n";
                text += $"Video length : {WebmPlayer.NaturalDuration.TimeSpan:mm\\:ss}\n";
                text += $"Video has audio : {(WebmPlayer.HasAudio ? "Yes" : "No")}\n";
                text += $"File size : {Folders.FileSize(WebmPlayer.Source.AbsolutePath.Replace("%20", " "))}\n";
            this.ShowMessageAsync(LbTitle.Content.ToString(), text);
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/BillyHennin/CsgoBackgroundChanger/releases");
            UpdateFlyOut.IsOpen = false;
        } 
            
    }
}