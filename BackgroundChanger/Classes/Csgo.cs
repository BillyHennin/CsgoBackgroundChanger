using System.Diagnostics;
using System.Windows;
using BackgroundChanger.Pages;

namespace BackgroundChanger.Classes
{
    public static class Csgo
    {
        public static bool ShowAlert(MainWindow windows, bool isClick = false)
        {
            if (Process.GetProcessesByName("csgo").Length != 0)
            {
                windows.LbAlertCsgo.Visibility = Visibility.Visible;
            }
            return windows.LbAlertCsgo.Visibility == Visibility.Visible;
        }
    }
}