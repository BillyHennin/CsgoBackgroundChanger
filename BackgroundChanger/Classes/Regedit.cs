using Microsoft.Win32;

namespace BackgroundChanger.Classes
{
    /// <summary>
    /// Class purpose : Handle the regedit
    /// </summary>
    public static class Regedit
    {
        //Regedit key
        private const string MyKey = @"SOFTWARE\CsgoBackgroundChanger";
        //Subkey references
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

        /// <summary>
        /// Function that check if the regedit keys has been initialized
        /// </summary>
        public static void CheckRegedit()
        {
            //Create key if it doesn't exist
            if (Registry.CurrentUser.OpenSubKey(MyKey, true) == null)
            {
                Registry.CurrentUser.CreateSubKey(MyKey);
            }
            //check if subkeys exist
            if (Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(MyWebmKey) == null)
            {
                SetValue(MyWebmKey, string.Empty);
            }

            if (Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(MyCsgoKey) == null)
            {
                SetValue(MyCsgoKey, string.Empty);
            }
            //set keys (empty if initialized)
            MyWebmFolderPath = Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(MyWebmKey).ToString();
            MyCsgoFolderPath = Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(MyCsgoKey).ToString();
        }
        //on set : update the regedit
        private static void SetValue(string key, object value)
            => Registry.CurrentUser.OpenSubKey(MyKey, true)?.SetValue(key, value);
        //on get : check the regedit
        private static string GetValue(string key) 
            => Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(key).ToString();
    }
}