using APIHub.Properties;
using MahApps.Metro;
using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace APIHub
{
    /// <summary>
    /// Interaction logic for ProductActivation.xaml
    /// </summary>
    public partial class ProductActivation:MetroWindow
    {
        public ProductActivation()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //=======THEME CONFIG (FROM MAINFORM)=======\\
            //ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Settings.Default.Theme), ThemeManager.GetAppTheme(Settings.Default.Scheme));
            //==========================================\\

            IDCode.Text = Environment.MachineName + "_" + Environment.UserName + "_";
            FirstName.Text = Environment.UserName;
            MessageBox.Show("Please Send your ID to CDAGaming on Discord, or in the PoGo Projects Channel to Receive your Product Key");

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgresult = MessageBox.Show(Properties.Resources.ExitButton_MSGBody, Properties.Resources.ExitButton_MSGCaption, MessageBoxButton.OK, MessageBoxImage.Exclamation);

            if (msgresult == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
            
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            String First_Name = FirstName.Text;
            String Last_Name = LastName.Text;
            String Licence_Key = LicenceKey.Text;
            String ID_Code = IDCode.Text;
            if (String.IsNullOrEmpty(First_Name) == false | String.IsNullOrEmpty(Last_Name) == false | String.IsNullOrEmpty(Licence_Key) == false)
            {
                Settings.Default.MachineID = ID_Code;
                Settings.Default.Name = First_Name + " " + Last_Name;
                Settings.Default.Save();
                string AuthCode = Properties.Resources.AuthCode;
                string AuthKey = Properties.Resources.AuthKey;
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

                if (Licence_Key.Contains("<_>" + Environment.MachineName + "<_>") | Licence_Key.Contains(AuthCode + AuthKey))
                {
                    MessageBox.Show(Properties.Resources.ActivationSuccess_MSGBody + Settings.Default.Name, Properties.Resources.ActivationSuccess_MSGCaption, MessageBoxButton.OK, MessageBoxImage.Information);
                    Settings.Default.Activated = true;
                    Settings.Default.Key = Licence_Key;
                    Settings.Default.Save();

                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBox.Show(Properties.Resources.IncorrectKey_MSGBody, Properties.Resources.Error_Caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show(Properties.Resources.EmptyArea_MSGBody, Properties.Resources.Error_Caption, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
