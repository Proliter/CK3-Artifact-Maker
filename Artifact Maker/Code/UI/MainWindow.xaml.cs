using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace ArtifactMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool advancedMode;
        public static string gamePath = "";

        public MainWindow()
        {
            advancedMode = bool.Parse(Config.get("use_advanced"));
            gamePath = readGamePath();

            InitializeComponent();
            applyLocalisation();

            frameMainWindow.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            contentcontrolMainForm.Content = frameMainWindow;
            buttonArtifactClick(buttonArtifact, new RoutedEventArgs());
        }

        private static string readGamePath()
        {
            string res = "";
            RegistryKey? registryUninstall;

            try
            {
                registryUninstall = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", false);
                if (registryUninstall == null)
                {
                    throw new Exception();
                }

                var subKey = registryUninstall.GetSubKeyNames();
                foreach (var item in subKey)
                {
                    if (item.Contains("Steam App "))
                    {
                        RegistryKey? temp = registryUninstall.OpenSubKey(item);
                        if (temp != null)
                        {
                            var displayName = temp.GetValue("DisplayName");
                            if (displayName != null && displayName.ToString() == "Crusader Kings III")
                            {
                                var tempPath = temp.GetValue("InstallLocation");
                                if (tempPath != null)
                                {
                                    string? tempString = tempPath.ToString();
                                    if (tempString != null)
                                    {
                                        res = tempString;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show(Localisation.get("MessageBox_registry") + "\r\n" + Localisation.get("MessageBox_not_affect_use"), Localisation.get("MessageBox_warnning"), MessageBoxButton.OK, MessageBoxImage.Warning);
                return "";
            }

            return res;
        }
    }
}
