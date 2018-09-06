using System;
using System.Diagnostics;
using System.Management;
using System.Windows;
using BackgroundChanger.Pages;
using System.Security.Principal;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Reflection;
using System.Threading.Tasks;

namespace BackgroundChanger.Classes
{
    public static class MonitorProcess
    {
        private static MainWindow window;

        public static async Task InitMonitoringAsync(MainWindow mainWindows)
        { 
            window = mainWindows;
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                if (principal.IsInRole(WindowsBuiltInRole.Administrator))
                {
                    ManagementEventWatcher ProcessStartEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStartTrace");
                    ManagementEventWatcher ProcessStopEvent = new ManagementEventWatcher("SELECT * FROM Win32_ProcessStopTrace");
                    ProcessStartEvent.EventArrived += ProcessStartEvent_EventArrived;
                    ProcessStopEvent.EventArrived += ProcessStopEvent_EventArrived;
                    try
                    {
                        ProcessStartEvent.Start();
                        ProcessStopEvent.Start();
                    }
                    catch
                    {
                        //How 'bout no
                    }
                } else
                {
                    if(!string.IsNullOrEmpty(Regedit.DontAskAdmin))
                    {
                        return;
                    }
                    var result = await window.ShowMessageAsync("Warning",
                        $"Unable to monitor if CSGO is open, please open {Assembly.GetExecutingAssembly().GetName().Name} with admin privilege",
                        MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings { NegativeButtonText = "Don't ask me again" } );
                    if(result == MessageDialogResult.Negative)
                    {
                        Regedit.DontAskAdmin = "1";
                    }
                }
            }
        }

        public static bool IsCSOpen() => Process.GetProcessesByName("csgo").Length != 0;

        private static void ProcessStartEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (IsCSOpen())
            {
                window.LbAlertCsgo.Invoke(new Action(() => window.LbAlertCsgo.Visibility = Visibility.Visible));
            }
        }

        private static void ProcessStopEvent_EventArrived(object sender, EventArrivedEventArgs e)
        {
            if (!IsCSOpen())
            {
                window.LbAlertCsgo.Invoke(new Action(() => window.LbAlertCsgo.Visibility = Visibility.Hidden));
            }
        }
    }
}