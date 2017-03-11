using APIHub.Properties;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System.Windows;

namespace APIHub
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow:MetroWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //=======THEME CONFIG (FROM MAINFORM)=======\\
            //ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Settings.Default.Theme), ThemeManager.GetAppTheme(Settings.Default.Scheme));
            //==========================================\\

            // Loads Version ID
            Version.Content = "V" + Settings.Default.CurrentVersion;
        }
    }
}
