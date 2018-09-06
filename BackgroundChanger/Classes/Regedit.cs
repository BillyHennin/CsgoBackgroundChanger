using Microsoft.Win32;
using System;
using System.Reflection;

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
        private const string MyAskAdmin = "NoAdmin";
        private const string CurrentVer = "CurrentVersion";

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

        public static string DontAskAdmin
        {
            get => GetValue(MyAskAdmin);
            set => SetValue(MyAskAdmin, value);
        }

        public static string CurrentVersion
        {
            get => GetValue(CurrentVer);
            set => SetValue(CurrentVer, value);
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

            if (Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(MyAskAdmin) == null)
            {
                SetValue(MyAskAdmin, string.Empty);
            }

            if (Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(CurrentVer) == null)
            {
                SetValue(CurrentVer, Update.GetVersion());
            }

            if (Update.CompareVerion(CurrentVersion))
            {
                CurrentVersion = Update.GetVersion();
                DontAskAdmin = string.Empty;
            }
        }
        //on set : update the regedit
        private static void SetValue(string key, object val)
            => Registry.CurrentUser.OpenSubKey(MyKey, true)?.SetValue(key, val);
        //on get : check the regedit
        private static string GetValue(string key) 
            => Registry.CurrentUser.OpenSubKey(MyKey, true)?.GetValue(key).ToString();
    }
}