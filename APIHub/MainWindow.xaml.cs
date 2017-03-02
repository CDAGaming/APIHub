using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using APIHub.Properties;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Text;
using MahApps.Metro;
using System.IO;
using System.Diagnostics;
using System.Timers;

namespace APIHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:MetroWindow
    {
        Stopwatch sw = new Stopwatch();
        Timer timer = new Timer();
        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbfont, uint cbfont, IntPtr pdv, [In] ref uint pcFonts);
        System.Drawing.FontFamily ff;
        Font font;
        int minutes = 00;
        int seconds = 00;
        int hours = 00;
        int milliseconds = 00;

        public object PrivateFontCollection { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Loads Custom Font
            LoadFont();

            // Stopwatch Timing

            //Settings.Default. = Convert.ToString(hours) + ":" + Convert.ToString(minutes) + ":" + Convert.ToString(seconds);
            //Settings.Default.Save();
            timer.Start();
            timer.Interval = 1000;
            timer.Elapsed += TimerTick;
            


            // Loads Start Web Page and Version + Activation Status
            Browser.Source = new Uri(Settings.Default.HighRPM);
            Version.Content = "V" + Settings.Default.CurrentVersion;

            if (Settings.Default.Activated == false)
            {
                ActivatedTag.Content = "Not Activated";
                ActivatedTag.Foreground = System.Windows.Media.Brushes.Red;
                Filter.IsEnabled = false;
                Browser.IsEnabled = false;
                SettingsButton.Content = "Activate";
                SettingsButton.Click += new RoutedEventHandler(SettingsButton_NotActivated);

                // Show Dialog on Launch
                ProductActivation frm = new ProductActivation();
                frm.ShowDialog();
            }
            else if (Settings.Default.Activated == true)
            {
                ActivatedTag.Content = "Activated";
                ActivatedTag.Foreground = System.Windows.Media.Brushes.Green;
                Filter.IsEnabled = true;
                Browser.IsEnabled = true;
                SettingsButton.Content = "Settings";
                SettingsButton.Click += new RoutedEventHandler(SettingsButton_Activated);

                if (Settings.Default.FontInstalled != true)
                {
                    MessageBoxResult msgresult = MessageBox.Show("This App Uses the Following Font: Montserrat-Regular. Would you like us to Install this Font?", "Font Installation", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

                    if (msgresult == MessageBoxResult.Yes)
                    {
                        InstallFont();
                    }
                    else if (msgresult == MessageBoxResult.No)
                    {
                        // Do Nothing But Ask on Next Launch
                    }
                }
                else
                {
                    // Do Nothing
                }
            }

            // Loads Themes & Filters
            Theme.ItemsSource = new List<string> { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" };
            Filter.ItemsSource = new List<string> { "LowRPM", "MediumRPM", "HighRPM", "AllRPM*" };

            //=============THEME & FILTER STARTUP & DEFAULTS CONFIGURATION=============\\
            Theme.SelectedValue = Settings.Default.DefaultTheme;
            Filter.SelectedValue = Settings.Default.DefaultFilter;

            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Settings.Default.DefaultTheme), ThemeManager.GetAppTheme(Settings.Default.Scheme));
        }

        private void TimerTick(object sender, ElapsedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                seconds = seconds + 1;

                if (seconds == 60)
                {
                    minutes = minutes + 1;
                    seconds = 0;
                    milliseconds = milliseconds + 60000;
                }
                if (minutes == 60)
                {
                    hours = hours + 1;
                    minutes = 0;
                }
                string Time = hours + ":" + minutes + ":" + seconds;
                AppTime.Content = "Time: " + Time;

                if (Settings.Default.LimitEnabled = true & milliseconds - Settings.Default.LimitTime == 1800000)
                {
                    Settings.Default.LimitEnabled = false;
                    Settings.Default.LimitTime = 0;
                    Settings.Default.Save();

                    Browser.IsEnabled = true;
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
            MessageBox.Show("This Feature will be in V1.2.0");
        }

        private void LoadFont()
        {
            byte[] fontArray = Properties.Resources.Montserrat_Regular;
            int dataLength = Properties.Resources.Montserrat_Regular.Length;

            IntPtr ptrData = Marshal.AllocCoTaskMem(dataLength);

            Marshal.Copy(fontArray, 0, ptrData, dataLength);

            uint cFonts = 0;

            AddFontMemResourceEx(ptrData, (uint)fontArray.Length, IntPtr.Zero, ref cFonts);

            PrivateFontCollection pfc = new PrivateFontCollection();

            pfc.AddMemoryFont(ptrData, dataLength);

            Marshal.FreeCoTaskMem(ptrData);

            ff = pfc.Families[0];
            font = new Font(ff, 15f, System.Drawing.FontStyle.Regular);
        }

        private void InstallFont()
        {
            if (Settings.Default.FontInstalled != true)
            {
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string FontPath = System.IO.Path.Combine(CurrentDirectory + @"Resources/Montserrat-Regular.ttf");

                Process.Start(FontPath);
                MessageBox.Show("Font will be Ready for use on Next Restart");
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This Feature will be Available in V1.2.0");
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

                Theme.Foreground = System.Windows.Media.Brushes.Red;
            }

            // Change Theme to Green

            if (selection == "Green")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Green"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Green";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Green;
            }

            // Change Theme to Blue

            if (selection == "Blue")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Blue"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Blue";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Blue;
            }

            // Change Theme to Purple

            if (selection == "Purple")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Purple"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Purple";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Purple;
            }

            // Change Theme to Orange

            if (selection == "Orange")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Orange"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Orange";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Orange;
            }

            // Change Theme to Lime

            if (selection == "Lime")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Lime"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Lime";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Lime;
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

                Theme.Foreground = System.Windows.Media.Brushes.Teal;
            }

            // Change Theme to Cyan

            if (selection == "Cyan")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Cyan"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Cyan";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Cyan;
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

                Theme.Foreground = System.Windows.Media.Brushes.Indigo;
            }

            // Change Theme to Violet

            if (selection == "Violet")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Violet"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Violet";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Violet;
            }

            // Change Theme to Pink

            if (selection == "Pink")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Pink"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Pink";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Pink;
            }

            // Change Theme to Magenta

            if (selection == "Magenta")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Magenta"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Magenta";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Magenta;
            }

            // Change Theme to Crimson

            if (selection == "Crimson")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Crimson"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Crimson";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Crimson;
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

                Theme.Foreground = System.Windows.Media.Brushes.Yellow;
            }

            // Change Theme to Brown

            if (selection == "Brown")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Brown"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Brown";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Brown;
            }

            // Change Theme to Olive

            if (selection == "Olive")
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent("Olive"), ThemeManager.GetAppTheme(Settings.Default.Scheme));
                Settings.Default.Theme = "Olive";
                Settings.Default.Save();

                Theme.Foreground = System.Windows.Media.Brushes.Olive;
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

                Theme.Foreground = System.Windows.Media.Brushes.Sienna;
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
            else if (Settings.Default.Filter == "AllRPM*")
            {
                MessageBox.Show("This Option will be Added in V1.2.0");
                Settings.Default.Filter = "HighRPM";
                Settings.Default.Save();
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sw.Stop();

        }

        private void SubmitKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Browser.Source = new Uri(Settings.Default.SubmitKey);
        }

        private void Browser_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl & Key.C | Key.RightCtrl & Key.C))
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    
                    if (Settings.Default.KeysObtained > 3)
                    {
                        Settings.Default.LimitEnabled = true;
                        Settings.Default.LimitTime = milliseconds;
                        Settings.Default.Save();

                        MessageBox.Show("Limit has been Reached, You can't obtain any keys for 30 Minutes");
                        Browser.IsEnabled = false;
                    }
                    else if (Settings.Default.KeysObtained <= 3)
                    {
                        Settings.Default.KeysObtained = +1;
                        Settings.Default.Save();
                        KeysObtained.Content = "Keys: " + Settings.Default.KeysObtained;
                    }
                });
            }
        }
    }
}
