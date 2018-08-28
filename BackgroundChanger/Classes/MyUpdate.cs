using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BackgroundChanger.Pages;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;

namespace BackgroundChanger.Classes
{
    public static class MyUpdate
    {
        private const string Url = "https://billyhennin.github.io/CsgoBackgroundChanger/";
        private const string Title = "Title";
        private const string Descr = "Description";
        private const string Versi = "Version";

        public static async Task<object> CheckUpdate(MainWindow window)
        {
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

                    window.UpdateFlyOut.IsOpen = true;
                    window.LbTitleUpdate.Content = string.IsNullOrEmpty(result[Title].ToString())
                            ? "A new update is available"
                            : result[Title].ToString();
                    window.LbDescUpdate.Content = string.IsNullOrEmpty(result[Descr].ToString())
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
        }
    }
}