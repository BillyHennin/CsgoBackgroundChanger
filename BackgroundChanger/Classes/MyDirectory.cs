using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace BackgroundChanger.Classes
{
    public static class MyDirectory
    {
        private const string DocFolderName = "CsgoBackgroundChanger";
        public static string[] GetAllWebm() => Directory.GetFiles(MyRegedit.MyWebmFolderPath, "*.webm");
        public static void CheckMyDocDir()
        {
            if (string.IsNullOrEmpty(MyRegedit.MyWebmFolderPath))
            {
                MyRegedit.MyWebmFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }

            if (MyRegedit.MyWebmFolderPath.Contains(DocFolderName))
            {
                return;
            }
            if (!Directory.GetDirectories(MyRegedit.MyWebmFolderPath).Contains(DocFolderName))
            {
                Directory.CreateDirectory(MyRegedit.MyWebmFolderPath + AddS(DocFolderName));
            }
            MyRegedit.MyWebmFolderPath = MyRegedit.MyWebmFolderPath + AddS(DocFolderName);
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
                    if (IsCsgoFolderValid())
                    {
                        return true;
                    }
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
            return csgoFiles.Contains(csgoFolder + AddS("csgo.exe")) && csgoFolders.Contains(csgoFolder + AddS("csgo")) &&
                   Directory.GetDirectories(csgoFolder + AddS("csgo")).Contains(csgoFolder + AddS("csgo", "panorama")) && 
                   Directory.GetDirectories(csgoFolder + AddS("csgo", "panorama")).Contains(csgoFolder + AddS("csgo","panorama", "videos"));
        }

        public static void UpdateBackground(string newFilePath)
        {

        }

        public static string RemoveUri(string fulluri, string optionalRemove = "")
        {
            optionalRemove = string.IsNullOrEmpty(optionalRemove) ? MyRegedit.MyWebmFolderPath.Replace("\\", "/") + "/" : optionalRemove;
            fulluri = fulluri.Replace("file:///", string.Empty).Replace(optionalRemove, string.Empty).Replace(".webm", string.Empty);
            return fulluri.First().ToString().ToUpper() + fulluri.Substring(1);
        }
        private static string AddS(string inS) => "\\" + inS;
        private static string AddS(params string[] insS) => insS.Aggregate(string.Empty, (current, inS) => current + AddS(inS));
    }
}