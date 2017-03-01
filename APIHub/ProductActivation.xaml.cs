using APIHub.Properties;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for ProductActivation.xaml
    /// </summary>
    public partial class ProductActivation
    {
        public ProductActivation()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //=======THEME CONFIG (FROM MAINFORM)=======\\
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Settings.Default.Theme), ThemeManager.GetAppTheme(Settings.Default.Scheme));
            //==========================================\\

            MachineCode.Text = Environment.MachineName;
            MessageBox.Show("Please Send your Machine ID to CDAGaming on Discord, or email chrisdoesawesomness@gmail.com, to receive your product key");

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("You'll be notified again when you restart the app to activate");
            Close();
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (FirstName.Text != "" & LastName.Text != "" & LicenceKey.Text != "")
            {
                Settings.Default.MachineID = MachineCode.Text;
                Settings.Default.Name = FirstName.Text + " " + LastName.Text;
                Settings.Default.Save();
                string LicenseKey = Settings.Default.MachineID + LicenceKey.Text;
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string KeyDirectory = System.IO.Path.Combine(CurrentDirectory + @"Resources/Auth.txt");

                if (File.Exists(KeyDirectory))
                {
                    string KeyData = File.ReadAllText(KeyDirectory);

                    if (LicenseKey == Settings.Default.MachineID + KeyData)
                    {
                        MessageBox.Show("Key Successfully Activated, " + Settings.Default.Name);
                        Settings.Default.Activated = true;
                        Settings.Default.Key = LicenseKey;
                        Settings.Default.Save();

                        Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        MessageBox.Show("Key is Incorrect, Please Try Again");
                    }
                }
                else
                {
                    MessageBox.Show("KeyDirectory doesn't Exist, Please Contact CDAGaming.");
                }
            }
            else
            {
                MessageBox.Show("One Value is Empty or Invalid, Please Try Again");
            }
        }
    }
}
