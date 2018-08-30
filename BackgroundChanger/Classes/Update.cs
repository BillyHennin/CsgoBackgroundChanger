//#if DEBUG Cause ReSharper to be retarded
// ReSharper disable All

using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BackgroundChanger.Pages;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;

namespace BackgroundChanger.Classes
{
    /// <summary>
    /// Class purpose : Check updates
    /// </summary>
    public static class Update
    {
        private const string Url = "https://billyhennin.github.io/CsgoBackgroundChanger/";
        private const string Title = "Title";
        private const string Descr = "Description";
        private const string Versi = "Version";

        public static async Task<object> CheckUpdate(MainWindow mainWindow)
        {
#if DEBUG
            //if we're in debug, no need to check for update
            return null;
#else
            mainWindow.BtnUpdate.Visibility = Visibility.Visible;
            var request = (HttpWebRequest) WebRequest.Create(Url);
            try
            {
                var response = request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    var result =
                            JObject.Parse(new StreamReader(stream ?? throw new InvalidOperationException(),
                                    Encoding.UTF8).ReadToEnd());
                    if (string.IsNullOrEmpty(result[Versi].ToString()))
                    {
                        throw new Exception();
                    }

                    if (Assembly.GetExecutingAssembly().GetName().Version
                                .CompareTo(new Version(result[Versi].ToString())) >= 0)
                    {
                        return null;
                    }
                    mainWindow.UpdateFlyOut.IsOpen = true;
                    mainWindow.LbTitleUpdate.Content = string.IsNullOrEmpty(result[Title].ToString())
                              ? "A new update is available"
                              : result[Title].ToString();
                    mainWindow.LbDescUpdate.Content = string.IsNullOrEmpty(result[Descr].ToString())
                              ? "Check the website for more infos."
                              : result[Descr].ToString();
                    return null;
                }
            }
            catch
            {
                await mainWindow.ShowMessageAsync("Unable to check for a new version",
                        "An error has occured while checking for an update. You can still check the website by yourself.");
                return null;
            }
#endif
        }
    }
}