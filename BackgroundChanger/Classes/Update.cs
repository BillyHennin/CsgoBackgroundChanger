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
        private const string Url = "https://billyhennin.github.io/CsgoBackgroundChanger/version.json";
        private const string Title = "Title";
        private const string Descr = "Description";
        private const string Versi = "Version";

        /// <summary>
        /// Allow the app to check if its up to date or if there's any new version available
        /// </summary>
        /// <param name="window">Main window of the app, will be used later on</param>
        /// <returns>Return null to show that the task is completed (dumb ikr)</returns>
        public static async Task<object> CheckUpdate(MainWindow window)
        {
#if DEBUG
            //if we're in debug, no need to check for update
            return null;
#else
            window.BtnUpdate.Visibility = Visibility.Visible;
            var request = (HttpWebRequest) WebRequest.Create(Url);
            if(!string.IsNullOrEmpty(Regedit.DontAskUpdate))
            {
                return null;
            }
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

                    if (CompareVerion(result[Versi].ToString()))
                    {
                        return null;
                    }
                    window.UpdateFlyOut.IsOpen = true;
                    window.LbTitleUpdate.Content = string.IsNullOrEmpty(result[Title].ToString())
                           ? "A new update is available"
                           : result[Title].ToString();
                    window.LbDescUpdate.Text = string.IsNullOrEmpty(result[Descr].ToString())
                           ? "Check the website for more infos."
                           : result[Descr].ToString();
                    return null;
                }
            }
            catch
            {
                await window.ShowMessageAsync("Unable to check for a new version",
                        "An error has occured while checking for an update. You can still check the website by yourself.");
                return null;
            }
#endif
        }

        public static string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static bool CompareVerion(string version)
            => Assembly.GetExecutingAssembly().GetName().Version.CompareTo(new Version(version)) >= 0;
    }
}