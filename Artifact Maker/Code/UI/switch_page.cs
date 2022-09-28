using System.Windows;
using System.Windows.Controls;

namespace ArtifactMaker
{
    interface MainPage
    {
        void applyLocalisation();
        void checkAdvancedMode(bool isAdvancedMode);
        void refreshItemsSource();
    }

    public partial class MainWindow
    {
        private string language = "";

        private Frame frameMainWindow = new Frame();
        private MainPage? pageArtifact = null;
        private PageHistoricalArtifact? pageHistoricalArtifact = null;
        private PageWiki? pageWiki = null;
        private MainPage? pageHelp = null;
        private MainPage? pageSettings = null;

        private void buttonArtifactClick(object sender, RoutedEventArgs e)
        {
            switchLeftButton(sender);

            if (pageArtifact == null)
            {
                pageArtifact = new PageArtifact();
            }

            pageArtifact.applyLocalisation();
            pageArtifact.checkAdvancedMode(advancedMode);
            pageArtifact.refreshItemsSource();

            frameMainWindow.Content = pageArtifact;
        }

        private void buttonHistoricalArtifactClick(object sender, RoutedEventArgs e)
        {
            switchLeftButton(sender);

            if (pageHistoricalArtifact == null)
            {
                pageHistoricalArtifact = new PageHistoricalArtifact();
            }

            frameMainWindow.Content = pageHistoricalArtifact;
        }

        private void buttonWikiClick(object sender, RoutedEventArgs e)
        {
            switchLeftButton(sender);

            if (pageWiki == null)
            {
                pageWiki = new PageWiki();
            }

            frameMainWindow.Content = pageWiki;
        }

        private void buttonHelpClick(object sender, RoutedEventArgs e)
        {
            switchLeftButton(sender);

            if (pageHelp == null)
            {
                pageHelp = new PageHelp();
            }

            pageHelp.applyLocalisation();

            frameMainWindow.Content = pageHelp;
        }

        private void buttonSettingsClick(object sender, RoutedEventArgs e)
        {
            switchLeftButton(sender);

            if (pageSettings == null)
            {
                pageSettings = new PageSettings();
            }

            pageSettings.applyLocalisation();
            pageSettings.checkAdvancedMode(advancedMode);

            frameMainWindow.Content = pageSettings;
        }

        private void buttonExitClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void switchLeftButton(object sender)
        {
            foreach (Button item in gridLeftButtons.Children)
            {
                item.IsEnabled = true;
            }
            ((Button)sender).IsEnabled = false;

            buttonExit.Content = Localisation.get("Button_Exit");
            applyLocalisation();
        }

        public void applyLocalisation()
        {
            if (Localisation.getLanguage() != language)
            {
                language = Localisation.getLanguage();

                Localisation.applyLocalisation(gridLeftButtons);
                Title = Localisation.get("MainWindow_title");
            }
        }
    }
}
