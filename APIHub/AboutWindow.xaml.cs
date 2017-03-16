using APIHub.Properties;
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
            // Loads Version ID
            NamePlusVersion.Content = "Licensed to: " + Settings.Default.Name + " - " + "V" + Settings.Default.CurrentVersion;
        }
    }
}
