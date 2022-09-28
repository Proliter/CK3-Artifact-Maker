using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ArtifactMaker
{
    /// <summary>
    /// Page_help.xaml 的交互逻辑
    /// </summary>
    public partial class PageHelp : Page, MainPage
    {
        private string language = "";

        public PageHelp()
        {
            InitializeComponent();
        }

        public void applyLocalisation()
        {
            textblockAuthor.Text = Localisation.get("TextBlock_Author");
            textblockGithub.Text = Localisation.get("TextBlock_Github");
            textblockVersion.Text = Localisation.get("TextBlock_Version");
            textblockVersionInfo.Text = Config.get("version");

            if (Localisation.getLanguage() != language)
            {
                language = Localisation.getLanguage();

                textboxHelp_0.Text = readInfo(0, 3);
                textboxHelp_1.Text = readInfo(1, 5);
            }

            return;

            static string readInfo(int helpNum, int lineNum)
            {
                string temp = "";
                for (int i = 0; i < lineNum; ++i)
                {
                    temp += Localisation.get("TextBox_Help_" + helpNum.ToString() + "_" + i.ToString()) + "\r\n";
                }
                return temp;
            }
        }

        public void checkAdvancedMode(bool isAdvancedMode)
        {
        }

        public void refreshItemsSource()
        {
        }

        private void hyperlinkGithubLinkClick(object sender, RoutedEventArgs e)
        {
            var link = new ProcessStartInfo
            {
                FileName = @"https://github.com/Proliter/CK3-Artifact-Maker",
                UseShellExecute = true
            };
            Process.Start(link);
        }
    }
}
