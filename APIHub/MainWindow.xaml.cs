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

namespace APIHub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow:MetroWindow
    {

        [DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbfont, uint cbfont, IntPtr pdv, [In] ref uint pcFonts);
        System.Drawing.FontFamily ff;
        Font font;

        public object PrivateFontCollection { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Loads Custom Font
            loadFont();

            // Loads Start Web Page and Version
            Browser.Source = new Uri(Settings.Default.HighRPM);
            Version.Content = "V" + Settings.Default.CurrentVersion;

            // Loads Themes & Filters
            Theme.ItemsSource = new List<string> { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" };
            Filter.ItemsSource = new List<string> { "LowRPM", "MediumRPM", "HighRPM", "AllRPM*" };

            //=============THEME & FILTER STARTUP & DEFAULTS CONFIGURATION=============\\
            Theme.SelectedValue = Settings.Default.DefaultTheme;
            Filter.SelectedValue = Settings.Default.DefaultFilter;

            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Settings.Default.DefaultTheme), ThemeManager.GetAppTheme(Settings.Default.Scheme));
        }

        private void loadFont()
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

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //===========THEME CONFIGURATION===========\\
            Tuple<AppTheme, Accent> appstyle = ThemeManager.DetectAppStyle(Application.Current);
            string selection = Convert.ToString(Theme.SelectedValue);
            Settings.Default.Theme = selection;
            Settings.Default.Save();

            
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
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
