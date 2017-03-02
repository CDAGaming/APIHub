using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using APIHub.Properties;
using Squirrel;

namespace APIHub
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            // Launch Window and Start Events
            base.OnStartup(e);

            //Sets Current Version and Resets Future Version
            Version CurrentVer = Assembly.GetExecutingAssembly().GetName().Version;
            string CurrentVersion = CurrentVer.ToString();
            Settings.Default.CurrentVersion = CurrentVersion;
            Settings.Default.Save();

            /* Auto Update Functionality
            using (var updater = await UpdateManager.GitHubUpdateManager("https://github.com/CDAGaming/APIHub", null, null, null, false, null))
            {
                var updatecheck = await updater.CheckForUpdate();

                if (updatecheck.ReleasesToApply.Any())
                {
                    List<ReleaseEntry> updatedownloads = updatecheck.ReleasesToApply;
                    string FutureVersion = updatecheck.FutureReleaseEntry.Version.ToString();
                    Version FutureVer = Version.Parse(FutureVersion);
                    Settings.Default.FutureVersion = FutureVersion;
                    Settings.Default.Save();

                    if (CurrentVer < FutureVer)
                    {
                        MessageBoxResult msgresult = MessageBox.Show("An Update is Available: " + "V" + CurrentVersion + " to V" + FutureVersion + ", Do you Wish to Update Now?", "New Update Available!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                        if (msgresult == MessageBoxResult.Yes)
                        {
                            await updater.DownloadReleases(updatedownloads);
                            await updater.ApplyReleases(updatecheck);
                            MessageBoxResult updateresult = MessageBox.Show("Update Complete, Please Relaunch Program to Start New Version.", "Update Completed!", MessageBoxButton.OK, MessageBoxImage.Information);

                            if (updateresult == MessageBoxResult.OK)
                            {
                                Application.Current.Shutdown();
                            }
                        }
                        else
                        {
                            // Update will show on Next Launch
                        }
                    }
                }
                else
                {
                    // Nothing Happens.
                }
            } // */
        }
    }
}
