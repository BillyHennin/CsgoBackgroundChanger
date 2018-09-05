using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace BackgroundChanger.Classes
{
    public static class Folders
    {
        private const string Name = "nuke";
        private const string DefError = "Incorrect CS:GO folder";
        private const string Exe = "csgo";
        private const string FilT = "webm";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllWebm()
        {
            if (!Directory.Exists(Regedit.MyWebmFolderPath) || string.IsNullOrEmpty(Regedit.MyWebmFolderPath))
            {
                Regedit.MyWebmFolderPath = $"{Application.StartupPath}{Format("Webms")}";
            }
            return !Directory.Exists(Regedit.MyWebmFolderPath) 
                    ? new string[0] 
                    : Directory.GetFiles(Regedit.MyWebmFolderPath, $"*.{FilT}").OrderBy(c => c).ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string UpdateWebmFolder()
        {
            using (var fbd = new FolderBrowserDialog { Description = @"Select your webm folder.", ShowNewFolderButton = true })
            {
                var result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    Regedit.MyWebmFolderPath = fbd.SelectedPath;
                }
            }
            return Regedit.MyWebmFolderPath;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="window"></param>
        /// <param name="firstTime"></param>
        /// <returns></returns>
        public static async Task<bool?> CheckMyCsgoDir(MetroWindow window, bool firstTime = true)
        {
            if (IsCsgoFolderValid(Regedit.MyCsgoFolderPath))
            {
                return true;
            }

            await window.ShowMessageAsync(DefError, "Please select your Counter-Strike Global Offensive Main folder");
            using (var fbd = new FolderBrowserDialog { Description = @"Select your Counter-Strike Global Offensive Main folder." })
            {
                var result = fbd.ShowDialog();
                if (result != DialogResult.OK || string.IsNullOrWhiteSpace(fbd.SelectedPath)) { return false; }
                Regedit.MyCsgoFolderPath = fbd.SelectedPath;
                return await CheckMyCsgoDir(window, false);
            }
        }

        /// <summary>
        /// Check if the path asserted is a valid csgo folder or not
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if valid</returns>
        public static bool IsCsgoFolderValid(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            var curFolder = $"{path}{Format(Exe)}";
            var result = Directory.GetFiles(path).Contains($"{curFolder}.exe");
                result = result && Directory.GetDirectories(path).Contains($"{curFolder}");            
                result = result && Directory.GetDirectories(curFolder).Contains($"{curFolder}{Format("panorama")}");
            curFolder += Format("panorama");
            return result && Directory.GetDirectories(curFolder).Contains($"{curFolder}{Format("videos")}");
        }

        public static async Task<bool> UpdateBackground(MetroWindow window, string path)
        {
            //if the csgo folder isn't valid, don't bother
            var isValid = await CheckMyCsgoDir(window);
            if (!isValid.Value)
            {
                return false;
            }
            var videoPath = $"{Regedit.MyCsgoFolderPath}{Format(Exe, "panorama", "videos")}";
            if (!Directory.GetFiles(videoPath).Contains($"{videoPath}{Format($"{Name}.{FilT}.tmp")}"))
            {
                //Save the original files
                File.Copy($"{videoPath}{Format($"{Name}.{FilT}")}", $"{videoPath}{Format($"{Name}.{FilT}.tmp")}");
                File.Copy($"{videoPath}{Format($"{Name}720.{FilT}")}", $"{videoPath}{Format($"{Name}720.{FilT}.tmp")}");
                File.Copy($"{videoPath}{Format($"{Name}540.{FilT}")}", $"{videoPath}{Format($"{Name}540.{FilT}.tmp")}");
            }
            //Update the background and overwrite the old ones
            File.Copy(path, $"{videoPath}{Format($"{Name}.{FilT}")}", true);
            File.Copy(path, $"{videoPath}{Format($"{Name}720.{FilT}")}", true);
            File.Copy(path, $"{videoPath}{Format($"{Name}540.{FilT}")}", true);
            return true;
        }

        public static string GetFileName(string uri)
        {
            //"BlaBlaBla\\mywebms\\nuke .webm" to "nuke .webm"
            uri = uri.Replace($"{Regedit.MyWebmFolderPath.Replace(@"\", "/")}/", string.Empty);
            //"nuke .webm" to "nuke"
            uri = uri.Replace("%20", " ").Replace($".{FilT}", string.Empty).Replace("-", " ").Replace("_", " ");
            //"nuke to Nuke"
            return $"{uri.First().ToString().ToUpper()}{uri.Substring(1)}";
        }

        /// <summary>
        /// Add "/" on the beginning fo the string (and in between)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string Format(params string[] str) => str.Aggregate(string.Empty, (curr, st) => $@"{curr}\{st}");

        public static string FileSize(string path)
        {
            var value = new FileInfo(path).Length;
            var sizeSuffixes = new []{ "bytes", "Ko", "Mo", "Go", "To", "Po", "Eo", "Zo", "Yo" };
            var mag = (int)Math.Log(value, 1024);
            var adjustedSize = (decimal)value / (1L << (mag * 10));
            if (Math.Round(adjustedSize, 1) < 1000)
            {
                return $"{adjustedSize:n1} {sizeSuffixes[mag]}";
            }
            adjustedSize /= 1024;
            return $"{adjustedSize:n1} {sizeSuffixes[mag + 1]}";
        }
    }
}