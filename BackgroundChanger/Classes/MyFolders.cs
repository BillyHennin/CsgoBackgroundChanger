using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace BackgroundChanger.Classes
{
    public static class MyFolders
    {
        private const string DocFolderName = "Webms";

        public static string[] GetAllWebm()
        {
            CheckWebmFolder();
            return Directory.GetFiles(MyRegedit.MyWebmFolderPath, "*.webm");
        }

        public static void CheckWebmFolder()
        {
            if (!Directory.Exists(MyRegedit.MyWebmFolderPath))
            {
                MyRegedit.MyWebmFolderPath = string.Empty;
            }

            if (MyRegedit.MyWebmFolderPath.Contains(DocFolderName))
            {
                return;
            }

            if (string.IsNullOrEmpty(MyRegedit.MyWebmFolderPath))
            {
                MyRegedit.MyWebmFolderPath = Application.StartupPath;
            }

            if (!Directory.GetDirectories(MyRegedit.MyWebmFolderPath).Contains(DocFolderName))
            {
                Directory.CreateDirectory(MyRegedit.MyWebmFolderPath + Format(DocFolderName));
            }

            MyRegedit.MyWebmFolderPath = MyRegedit.MyWebmFolderPath + Format(DocFolderName);
        }

        public static string UpdateWebFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    MyRegedit.MyCsgoFolderPath = fbd.SelectedPath;
                    //await CheckMyCsgoDir(window, false);
                }
                
            }
            //TODO : Ask new folder
            //MyRegedit.MyWebmFolderPath = newFolder;



            return MyRegedit.MyWebmFolderPath;
        }

        public static async Task<bool?> CheckMyCsgoDir(MetroWindow window, bool firstTime = true)
        {
            if (IsCsgoFolderValid())
            {
                return true;
            }

            await window.ShowMessageAsync("Incorrect CS:GO folder", "Please select your CS:GO folder");
            using (var fbd = new FolderBrowserDialog())
            {
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    MyRegedit.MyCsgoFolderPath = fbd.SelectedPath;
                    await CheckMyCsgoDir(window, false);
                }
            }

            if (!firstTime)
            {
                return null;
            }

            await window.ShowMessageAsync("Error !", "This application cannot work without a specified CS:GO folder.");
            return false;
        }

        private static bool IsCsgoFolderValid()
        {
            var csgoFolder = MyRegedit.MyCsgoFolderPath;
            if (string.IsNullOrEmpty(csgoFolder))
            {
                return false;
            }

            var csgoFolders = Directory.GetDirectories(csgoFolder);
            var csgoFiles = Directory.GetFiles(csgoFolder);
            return csgoFiles.Contains(csgoFolder + Format("csgo.exe")) &&
                   csgoFolders.Contains(csgoFolder + Format("csgo")) &&
                   Directory.GetDirectories(csgoFolder + Format("csgo"))
                       .Contains(csgoFolder + Format("csgo", "panorama")) &&
                   Directory.GetDirectories(csgoFolder + Format("csgo", "panorama"))
                       .Contains(csgoFolder + Format("csgo", "panorama", "videos"));
        }

        public static async Task<bool> UpdateBackground(MetroWindow window, string newFilePath)
        {
            //TODO : Check everything
            var isValid = await CheckMyCsgoDir(window);
            if (!isValid.Value)
            {
                return false;
            }

            var videoPath = MyRegedit.MyCsgoFolderPath + Format("csgo", "panorama", "videos");
            if (!Directory.GetFiles(videoPath).Contains(videoPath + Format("nuke.webm.tmp")))
            {
                File.Copy(videoPath + Format("nuke.webm"), videoPath + Format("nuke.webm.tmp"));
                File.Copy(videoPath + Format("nuke720.webm"), videoPath + Format("nuke720.webm.tmp"));
                File.Copy(videoPath + Format("nuke540.webm"), videoPath + Format("nuke540.webm.tmp"));
            }

            File.Delete(videoPath + Format("nuke.webm"));
            File.Delete(videoPath + Format("nuke720.webm"));
            File.Delete(videoPath + Format("nuke540.webm"));

            var webmPath = MyRegedit.MyWebmFolderPath + Format(newFilePath) + ".webm";
            File.Copy(webmPath, videoPath + Format("nuke.webm"));
            File.Copy(webmPath, videoPath + Format("nuke720.webm"));
            File.Copy(webmPath, videoPath + Format("nuke540.webm"));
            return true;
        }

        public static string RemoveUri(string fulluri, string optionalRemove = "")
        {
            optionalRemove = string.IsNullOrEmpty(optionalRemove)
                ? MyRegedit.MyWebmFolderPath.Replace("\\", "/") + "/"
                : optionalRemove;
            fulluri = fulluri.Replace("file:///", string.Empty).Replace(optionalRemove, string.Empty)
                .Replace(".webm", string.Empty);
            return fulluri.First().ToString().ToUpper() + fulluri.Substring(1);
        }

        private static string Format(params string[] insS) =>
            insS.Aggregate(string.Empty, (current, inS) => current + "\\" + inS);
    }
}