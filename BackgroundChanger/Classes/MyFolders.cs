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
        private const string Name = "nuke";
        private const string DefError = "Incorrect CS:GO folder";
        private const string Exe = "csgo";
        private const string FilT = "webm";

        public static string[] GetAllWebm()
        {
            CheckWebmFolder();
            return Directory.GetFiles(MyRegedit.MyWebmFolderPath, $"*.{FilT}");
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
            using (var fbd =
                    new FolderBrowserDialog {Description = @"Select your webm folder.", ShowNewFolderButton = true})
            {
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    MyRegedit.MyCsgoFolderPath = fbd.SelectedPath;
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

            await window.ShowMessageAsync(DefError, "Please select your CS:GO folder");
            using (var fbd = new FolderBrowserDialog {Description = @"Select your CS:GO folder."})
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

            await window.ShowMessageAsync(DefError, "This application cannot work without a specified CS:GO folder.");
            return false;
        }

        public static bool IsCsgoFolderValid()
        {
            var csgoFolder = MyRegedit.MyCsgoFolderPath;
            if (string.IsNullOrEmpty(csgoFolder))
            {
                return false;
            }

            var csgoFolders = Directory.GetDirectories(csgoFolder);
            var csgoFiles = Directory.GetFiles(csgoFolder);
            return csgoFiles.Contains($"{csgoFolder}{Format($"{Exe}.exe")}") &&
                   csgoFolders.Contains($"{csgoFolder}{Format(Exe)}") &&
                   Directory.GetDirectories($"{csgoFolder}{Format(Exe)}")
                           .Contains($"{csgoFolder}{Format(Exe, "panorama")}") && Directory
                           .GetDirectories($"{csgoFolder}{Format(Exe, "panorama")}")
                           .Contains($"{csgoFolder}{Format(Exe, "panorama", "videos")}");
        }

        public static async Task<bool> UpdateBackground(MetroWindow window, string newFilePath)
        {
            //TODO : Check everything
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

        public static string RemoveUri(string fulluri, string optionalRemove = "")
        {
            optionalRemove = string.IsNullOrEmpty(optionalRemove)
                    ? $"{MyRegedit.MyWebmFolderPath.Replace("\\", "/")}/"
                    : optionalRemove;
            fulluri = fulluri.Replace("file:///", string.Empty).Replace(optionalRemove, string.Empty)
                    .Replace($".{FilT}", string.Empty);
            return $"{fulluri.First().ToString().ToUpper()}{fulluri.Substring(1)}";
        }

        private static string Format(params string[] str)
        {
            return str.Aggregate(string.Empty, (curr, st) => $"{curr}\\{st}");
        }
    }
}