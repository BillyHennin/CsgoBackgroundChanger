using Microsoft.Win32;

namespace BackgroundChanger.Classes
{
    public static class MyRegedit
    {
        private const string MyKey = @"SOFTWARE\CsgoBackgroundChanger";
        private const string MyWebmKey = "WebmFolder";
        private const string MyCsgoKey = "CsgoFolder";

        public static string MyWebmFolderPath
        {
            get => GetValue(MyWebmKey);
            set => SetValue(MyWebmKey, value);
        }
        public static string MyCsgoFolderPath
        {
            get => GetValue(MyCsgoKey);
            set => SetValue(MyCsgoKey, value);
        }
        public static void CheckRegedit()
        {
            if (Registry.CurrentUser.OpenSubKey(MyKey, true) == null)
            {
                Registry.CurrentUser.CreateSubKey(MyKey);
            }

            if (Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(MyWebmKey) == null)
            {
                SetValue(MyWebmKey, string.Empty);
            }

            if (Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(MyCsgoKey) == null)
            {
                SetValue(MyCsgoKey, string.Empty);
            }
            MyWebmFolderPath = Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(MyWebmKey).ToString();
            MyCsgoFolderPath = Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(MyCsgoKey).ToString();
        }
        private static void SetValue(string key, string value) => Registry.CurrentUser.OpenSubKey(MyKey, true)?.SetValue(key, value);
        private static string GetValue(string key) => Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(key).ToString();
    }
}