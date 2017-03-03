using APIHub.Properties;
using MahApps.Metro;
using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace APIHub
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //=======THEME CONFIG (FROM MAINFORM)=======\\
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Settings.Default.Theme), ThemeManager.GetAppTheme(Settings.Default.Scheme));
            //==========================================\\
        }
    }
}
