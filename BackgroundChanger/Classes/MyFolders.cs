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
        private const string Name = "nuke";
        private const string DefError = "Incorrect CS:GO folder";
        private const string Exe = "csgo";
        private const string FilT = "webm";

        public static string[] GetAllWebm()
        {
            if (!Directory.Exists(MyRegedit.MyWebmFolderPath) || string.IsNullOrEmpty(MyRegedit.MyWebmFolderPath))
            {
                MyRegedit.MyWebmFolderPath = $"{Application.StartupPath}{Format("Webms")}";
            }
            return Directory.GetFiles(MyRegedit.MyWebmFolderPath, $"*.{FilT}");
        }
        

        public static string UpdateWebFolder()
        {
            using (var fbd = new FolderBrowserDialog {Description = @"Select your webm folder.", ShowNewFolderButton = true})
            {
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    MyRegedit.MyWebmFolderPath = fbd.SelectedPath;
                }
            }

            return MyRegedit.MyWebmFolderPath;
        }

        public static async Task<bool?> CheckMyCsgoDir(MetroWindow window, bool firstTime = true)
        {
            if (IsCsgoFolderValid())
            {
                return true;
            }

            await window.ShowMessageAsync(DefError, "Please select your Counter-Strike Global Offensive Main folder");
            using (var fbd = new FolderBrowserDialog {Description = @"Select your Counter-Strike Global Offensive Main folder." })
            {
                var result = fbd.ShowDialog();
                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    return false;
                }
                MyRegedit.MyCsgoFolderPath = fbd.SelectedPath;
                return await CheckMyCsgoDir(window, false);
            }
        }

        public static bool IsCsgoFolderValid(string strPath = "")
        {
            var csgoFolder = string.IsNullOrEmpty(strPath) ? MyRegedit.MyCsgoFolderPath : strPath;
            if (string.IsNullOrEmpty(csgoFolder))
            {
                return false;
            }
            var curFolder = $"{csgoFolder}{Format(Exe)}";
            var result = Directory.GetFiles(csgoFolder).Contains($"{curFolder}.exe");
            result = result && Directory.GetDirectories(csgoFolder).Contains($"{curFolder}");            
            result = result && Directory.GetDirectories(curFolder).Contains($"{curFolder}{Format("panorama")}");
            curFolder += Format("panorama");
            return result && Directory.GetDirectories(curFolder).Contains($"{curFolder}{Format("videos")}");
        }

        public static async Task<bool> UpdateBackground(MetroWindow window, string newFilePath)
        {
            var isValid = await CheckMyCsgoDir(window);
            if (!isValid.Value)
            {
                return false;
            }

            var videoPath = $"{MyRegedit.MyCsgoFolderPath}{Format(Exe, "panorama", "videos")}";
            if (!Directory.GetFiles(videoPath).Contains($"{videoPath}{Format($"{Name}.{FilT}.tmp")}"))
            {
                File.Copy($"{videoPath}{Format($"{Name}.webm")}", $"{videoPath}{Format($"{Name}.{FilT}.tmp")}");
                File.Copy($"{videoPath}{Format($"{Name}720.webm")}", $"{videoPath}{Format($"{Name}720.{FilT}.tmp")}");
                File.Copy($"{videoPath}{Format($"{Name}540.webm")}", $"{videoPath}{Format($"{Name}540.{FilT}.tmp")}");
            }

            File.Delete($"{videoPath}{Format($"{Name}.{FilT}")}");
            File.Delete($"{videoPath}{Format($"{Name}720.{FilT}")}");
            File.Delete($"{videoPath}{Format($"{Name}540.{FilT}")}");

            var webmPath = $"{MyRegedit.MyWebmFolderPath}{Format(newFilePath)}.{FilT}";
            File.Copy(webmPath, $"{videoPath}{Format($"{Name}.{FilT}")}");
            File.Copy(webmPath, $"{videoPath}{Format($"{Name}720.{FilT}")}");
            File.Copy(webmPath, $"{videoPath}{Format($"{Name}540.{FilT}")}");
            return true;
        }

        public static string RemoveUri(string fulluri, string remove = "")
        {
            remove = string.IsNullOrEmpty(remove)
                    ? $"{MyRegedit.MyWebmFolderPath.Replace("\\", "/")}/"
                    : remove;
            fulluri = fulluri.Replace("file:///", string.Empty).Replace(remove, string.Empty)
                    .Replace($".{FilT}", string.Empty);
            return $"{fulluri.First().ToString().ToUpper()}{fulluri.Substring(1)}";
        }

        private static string Format(params string[] str) 
            => str.Aggregate(string.Empty, (curr, st) => $"{curr}\\{st}");
    }
}