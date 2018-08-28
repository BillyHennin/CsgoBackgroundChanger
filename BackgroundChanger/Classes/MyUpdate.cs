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

        public static async Task<object> CheckUpdate(MainWindow window)
        {
            var request = (HttpWebRequest) WebRequest.Create(Url);
            try
            {
                var response = request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    var result =
                        JObject.Parse(new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.UTF8)
                            .ReadToEnd());
                    if (string.IsNullOrEmpty(result["Version"].ToString()))
                    {
                        throw new Exception();
                    }

                    if (Assembly.GetExecutingAssembly().GetName().Version
                            .CompareTo(new Version(result["Version"].ToString())) >= 0)
                    {
                        return null;
                    }

                    window.UpdateFlyOut.IsOpen = true;
                    window.LbTitleUpdate.Content = string.IsNullOrEmpty(result["Title"].ToString())
                        ? "A new update is available"
                        : result["Title"].ToString();
                    window.LbDescUpdate.Content = string.IsNullOrEmpty(result["Description"].ToString())
                        ? "Check the website for more infos."
                        : result["Description"].ToString();
                    return null;
                }
            }
            catch
            {
                await window.ShowMessageAsync("Error", "An error has occured while checking for an update.");
                return null;
            }
        }
    }
}