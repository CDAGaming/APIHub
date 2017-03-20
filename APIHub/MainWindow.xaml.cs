using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using APIHub.Properties;
using System.Runtime.InteropServices;
using MahApps.Metro;
using System.Diagnostics;
using System.Timers;
using Microsoft.Win32.SafeHandles;

namespace APIHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:MetroWindow, IDisposable
    {

        Timer timer = new Timer();
        SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);
        int minutes = 00;
        int seconds = 00;
        int hours = 00;
        int milliseconds = 00;
        string Keys;
        int KeyAmount = 00;
        bool disposed = false;
        bool CopyPaste = false;
        Color Red = (Color)ColorConverter.ConvertFromString("#D50000");
        Color Green = (Color)ColorConverter.ConvertFromString("#00E676");
        Color Yellow = (Color)ColorConverter.ConvertFromString("#FF6F00");

        public object PrivateFontCollection { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Stopwatch Timing
            timer.Start();
            timer.Interval = 250;
            timer.Elapsed += TimerTick;
            
            // Loads Start Web Page and Version + Activation Status
            Browser.Source = new Uri(Settings.Default.AllRPM);
            Version.Content = "V" + Settings.Default.CurrentVersion;

            if (Settings.Default.Activated == false)
            {
                ActivatedTag.Content = "Not Activated";
                ActivatedTag.Foreground = new SolidColorBrush(Red);
                Filter.IsEnabled = false;
                Browser.IsEnabled = false;
                Browser.Visibility = 0;
                SettingsButton.Content = "Activate";
                SettingsButton.Click += new RoutedEventHandler(SettingsButton_NotActivated);

                // Show Dialog on Launch
                ProductActivation frm = new ProductActivation();
                frm.ShowDialog();
            }
            else if (Settings.Default.Activated == true)
            {
                ActivatedTag.Content = "Activated";
                ActivatedTag.Foreground = new SolidColorBrush(Green);
                Filter.IsEnabled = true;
                Browser.IsEnabled = true;
                SettingsButton.Content = "Settings";
                SettingsButton.Click += new RoutedEventHandler(SettingsButton_Activated);

                /*if (Settings.Default.FontInstalled != true)
                {
                    MessageBoxResult msgresult = MessageBox.Show(Properties.Resources.MSG_FontInstall_Body, Properties.Resources.MSG_FontInstall_Caption, MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                    if (msgresult == MessageBoxResult.Yes)
                    {
                        InstallFont();
                        Settings.Default.FontInstalled = true;
                        Settings.Default.Save();
                    }
                    else if (msgresult == MessageBoxResult.No)
                    {
                        // Do Nothing But Will Ask on Next Launch
                    }
                }
                else
                {
                    // Do Nothing
                }*/
            }

            // Loads Key Amount
            Keys = String.Format("{0:00}", KeyAmount);
            KeysObtained.Content = "Keys: " + Keys;
            KeysObtained.Foreground = new SolidColorBrush(Green);

            // Loads Themes & Filters
            Theme.ItemsSource = new List<string> { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" };
            Filter.ItemsSource = new List<string> { "LowRPM", "MediumRPM", "HighRPM", "AllRPM" };

            //=============THEME & FILTER STARTUP & DEFAULTS CONFIGURATION=============\\
            Theme.SelectedValue = Settings.Default.DefaultTheme;
            Filter.SelectedValue = Settings.Default.DefaultFilter;

            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Settings.Default.DefaultTheme), ThemeManager.GetAppTheme(Settings.Default.Scheme));
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(delegate
            {
                milliseconds = milliseconds + 250;

                if (milliseconds == 1000)
                {
                    seconds = seconds + 1;
                    milliseconds = 0;
                }

                if (seconds == 60)
                {
                    minutes = minutes + 1;
                    seconds = 0;
                }
                if (minutes == 60)
                {
                    hours = hours + 1;
                    minutes = 0;
                }

                string Time = String.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                AppTime.Content = "Time: " + Time;

                // Detect If CTRL + C has been Pressed & Determine Limit Status
                if (CopyPaste == true)
                {
                    if (Settings.Default.LimitEnabled == false)
                    {
                        KeyAmount = KeyAmount + 1;
                        Keys = String.Format("{0:00}", KeyAmount);
                        KeysObtained.Content = "Keys: " + Keys;
                        KeysObtained.Foreground = new SolidColorBrush(Green);

                        if (KeyAmount >= 3 && KeyAmount != 5)
                        {
                            KeysObtained.Foreground = new SolidColorBrush(Yellow);
                        }
                        else if (KeyAmount == 5)
                        {
                            Settings.Default.LimitEnabled = true;
                            KeysObtained.Foreground = new SolidColorBrush(Red);
                            KeysObtained.Content = "LIMIT ACTIVE!";
                        }
                        CopyPaste = false;
                    }
                    else if (Settings.Default.LimitEnabled == true & CopyPaste == true)
                    {
                        CopyPaste = false;
                        MessageBox.Show("Please Wait " + Settings.Default.Reset_Minutes + " Minutes for the limit to expire");
                        Clipboard.Clear();
                    }
                }

                // If minutes App has been Run is Equal to the 5 Minutes past prior Refreshes
                // 5*1=5, 5*2=10, 5*3=15,etc

                if (minutes == Settings.Default.Reset_Minutes * Settings.Default.Reset_Times)
                {
                    if (Settings.Default.Filter == "LowRPM")
                    {
                        Browser.Source = new Uri(Settings.Default.LowRPM);
                    }
                    else if (Settings.Default.Filter == "MediumRPM")
                    {
                        Browser.Source = new Uri(Settings.Default.MediumRPM);
                    }
                    else if (Settings.Default.Filter == "HighRPM")
                    {
                        Browser.Source = new Uri(Settings.Default.HighRPM);
                    }
                    else if (Settings.Default.Filter == "AllRPM")
                    {
                        Browser.Source = new Uri(Settings.Default.AllRPM);
                    }

                    if (Settings.Default.LimitEnabled == true)
                    {
                        Settings.Default.Reset_Minutes = Settings.Default.Reset_Minutes * 2;
                        Settings.Default.Reset_Times = Settings.Default.Reset_Times + 1;
                        Settings.Default.LimitEnabled = false;
                        Settings.Default.Save();

                        KeyAmount = 0;
                        Keys = String.Format("{0:00}", KeyAmount);
                        KeysObtained.Content = "Keys: " + Keys;
                        KeysObtained.Foreground = new SolidColorBrush(Green);
                    }
                }
            });
        }

        private void SettingsButton_NotActivated(object sender, RoutedEventArgs e)
        {
            ProductActivation frm = new ProductActivation();
            frm.ShowDialog();
        }

        private void SettingsButton_Activated(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();
        }

        /*private void InstallFont()
        {
            if (Settings.Default.FontInstalled != true)
            {
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string FontPath = System.IO.Path.Combine(CurrentDirectory + @"Resources/Montserrat-Regular.ttf");

                Process.Start(FontPath);
            }
        }*/

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutpg = new AboutWindow();
            aboutpg.ShowDialog();
        }

        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //===========THEME CONFIGURATION===========\\
            Tuple<AppTheme, Accent> appstyle = ThemeManager.DetectAppStyle(Application.Current);
            string selection = Convert.ToString(Theme.SelectedValue);
            Settings.Default.Theme = selection;
            Settings.Default.Save();

            // Change Theme to Red

            if (selection == "Red")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Red"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Red";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Red;
            }

            // Change Theme to Green

            if (selection == "Green")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Green"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Green";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Green;
            }

            // Change Theme to Blue

            if (selection == "Blue")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Blue"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Blue";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Blue;
            }

            // Change Theme to Purple

            if (selection == "Purple")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Purple"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Purple";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Purple;
            }

            // Change Theme to Orange

            if (selection == "Orange")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Orange"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Orange";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Orange;
            }

            // Change Theme to Lime

            if (selection == "Lime")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Lime"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Lime";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Lime;
            }

            // Change Theme to Emerald

            if (selection == "Emerald")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Emerald"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Emerald";
                Settings.Default.Save();

                Theme.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(85, 212, 63));
            }

            // Change Theme to Teal

            if (selection == "Teal")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Teal"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Teal";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Teal;
            }

            // Change Theme to Cyan

            if (selection == "Cyan")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Cyan"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Cyan";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Cyan;
            }

            // Change Theme to Cobalt

            if (selection == "Cobalt")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Cobalt"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Cobalt";
                Settings.Default.Save();

                Theme.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 71, 171));
            }

            // Change Theme to Indigo

            if (selection == "Indigo")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Indigo"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Indigo";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Indigo;
            }

            // Change Theme to Violet

            if (selection == "Violet")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Violet"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Violet";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Violet;
            }

            // Change Theme to Pink

            if (selection == "Pink")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Pink"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Pink";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Pink;
            }

            // Change Theme to Magenta

            if (selection == "Magenta")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Magenta"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Magenta";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Magenta;
            }

            // Change Theme to Crimson

            if (selection == "Crimson")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Crimson"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Crimson";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Crimson;
            }

            // Change Theme to Amber

            if (selection == "Amber")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Amber"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Amber";
                Settings.Default.Save();

                Theme.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 191, 0));
            }

            // Change Theme to Yellow

            if (selection == "Yellow")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Yellow"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Yellow";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Yellow;
            }

            // Change Theme to Brown

            if (selection == "Brown")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Brown"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Brown";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Brown;
            }

            // Change Theme to Olive

            if (selection == "Olive")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Olive"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Olive";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Olive;
            }

            // Change Theme to Steel

            if (selection == "Steel")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Steel"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Steel";
                Settings.Default.Save();

                Theme.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(224, 223, 219));
            }

            // Change Theme to Mauve

            if (selection == "Mauve")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Mauve"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Mauve";
                Settings.Default.Save();

                Theme.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(224, 126, 255));
            }

            // Change Theme to Taupe

            if (selection == "Taupe")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Taupe"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Taupe";
                Settings.Default.Save();

                Theme.Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(103, 76, 71));
            }

            // Change Theme to Sienna

            if (selection == "Sienna")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Sienna"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Sienna";
                Settings.Default.Save();

                Theme.Foreground = Brushes.Sienna;
            }
        }
        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //===========FILTER CONFIGURATION===========\\
            string selection = Convert.ToString(Filter.SelectedValue);
            Settings.Default.Filter = selection;
            Settings.Default.Save();

            if (Settings.Default.Filter == "LowRPM")
            {
                Browser.Source = new Uri(Settings.Default.LowRPM);
            }
            else if (Settings.Default.Filter == "MediumRPM")
            {
                Browser.Source = new Uri(Settings.Default.MediumRPM);
            }
            else if (Settings.Default.Filter == "HighRPM")
            {
                Browser.Source = new Uri(Settings.Default.HighRPM);
            }
            else if (Settings.Default.Filter == "AllRPM")
            {
                Browser.Source = new Uri(Settings.Default.AllRPM);
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (disposed == false)
            {
                Dispose();
            }
            else
            {
                disposed = false;
            }
            timer.Stop();
            Application.Current.Shutdown();
        }

        private void SubmitKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Browser.Source = new Uri(Settings.Default.SubmitKey);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool Disposing)
        {
            if (disposed)
            {
                return;
            }
            if (Disposing)
            {
                handle.Dispose();
                ((IDisposable)timer).Dispose();
            }
            disposed = true;
        }

        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control)
            {
                CopyPaste = true;
            }
        }

        private void Browser_ShowContextMenu(object sender, Awesomium.Core.ContextMenuEventArgs e)
        {
            e.Handled = true;
        }
    }
}