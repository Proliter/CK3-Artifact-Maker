using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

/*
 this page used Ookii.Dialogs.Wpf
 https://github.com/ookii-dialogs/ookii-dialogs-wpf

 Ookii.Dialogs.Wpf License: BSD 3-Clause License
 
BSD 3-Clause License

Copyright (c) C. Augusto Proiete 2018-2021
Copyright (c) Sven Groot         2009-2018
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

*/

namespace ArtifactMaker
{
    /// <summary>
    /// Page_settings.xaml 的交互逻辑
    /// </summary>
    public partial class PageSettings : Page, MainPage
    {
        private string language = "";
        private bool? advancedMode;

        private List<string> dataLanguageList = new List<string>();
        private List<string> dataAdvancedList = new List<string>();

        private List<string> localisationLanguageList = new List<string>();
        private List<string> localisationAdvancedList = new List<string>();

        public PageSettings()
        {
            InitializeComponent();

            dataLanguageList = Data.get("language_list");
            dataAdvancedList = Data.get("advanced_mode_list");

            string path = Config.get("game_document_path");
            if (path.Contains("%userProfile%"))
            {
                path = Environment.ExpandEnvironmentVariables(path);
            }
            textboxSavePath.Text = path;

            textboxGamePath.Text = MainWindow.gamePath;
            textboxSaveFileName.Text = Config.get("game_run_file_name");
            textboxSaveFileName.IsReadOnly = true;
        }

        public void applyLocalisation()
        {
            if (Localisation.getLanguage() != language)
            {
                language = Localisation.getLanguage();


                Localisation.applyLocalisation(gridPageSettings);

                Localisation.loadLocalisationFirstLevel(dataLanguageList, localisationLanguageList, "language_");
                Localisation.loadLocalisationFirstLevel(dataAdvancedList, localisationAdvancedList);

                refreshItemsSource();
            }

            int targetLanguage = dataLanguageList.FindIndex(a => a == language.Replace('-', '_'));
            if (targetLanguage != comboboxLanguage.SelectedIndex)
            {
                comboboxLanguage.SelectedIndex = targetLanguage;
            }
        }

        public void checkAdvancedMode(bool isAdvancedMode)
        {
            advancedMode = isAdvancedMode;

            int targetAdvanced;
            if ((bool)advancedMode)
            {
                targetAdvanced = dataAdvancedList.FindIndex(a => a == "setting_advanced_on");
            }
            else
            {
                targetAdvanced = dataAdvancedList.FindIndex((a) => a == "setting_advanced_off");
            }

            if (targetAdvanced != comboboxAdvanced.SelectedIndex)
            {
                comboboxAdvanced.SelectedIndex = targetAdvanced;
            }
        }

        public void refreshItemsSource()
        {
            comboboxLanguage.ItemsSource = localisationLanguageList;
            comboboxLanguage.Items.Refresh();
            comboboxAdvanced.ItemsSource = localisationAdvancedList;
            comboboxAdvanced.Items.Refresh();
        }

        private void comboboxLanguageSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1)
            {
                return;
            }

            string source = dataLanguageList[((ComboBox)sender).SelectedIndex];

            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(source.Replace('_', '-'));

            if (Localisation.getLanguage() == System.Threading.Thread.CurrentThread.CurrentUICulture.Name)
            {
                return;
            }

            Localisation.changeLanguage();

            if (Data.version != 0)
            {
                AnalyzeGameDataFiles.applyAdaption();
            }

            applyLocalisation();
            if (advancedMode != null)
            {
                checkAdvancedMode((bool)advancedMode);
            }
            //MainWindow.applyLocalisation();
        }

        private void comboboxAdvancedSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex == -1)
            {
                return;
            }

            string source = dataAdvancedList[((ComboBox)sender).SelectedIndex];

            switch (source)
            {
                case "setting_advanced_on":
                    MainWindow.advancedMode = true;
                    break;
                case "setting_advanced_off":
                    MainWindow.advancedMode = false;
                    break;
                default:
                    break;
            }
        }

        private void buttonSavePathClick(object sender, RoutedEventArgs e)
        {
            //懒狗微软
            var folder = new VistaFolderBrowserDialog();
            if (folder.ShowDialog() == true)
            {
                string path = folder.SelectedPath + @"\";

                textboxSavePath.Text = path;
                Config.change("game_document_path", path);
            }
        }

        private void buttonSaveFileNameClick(object sender, RoutedEventArgs e)
        {
            if (textboxSaveFileName.IsReadOnly == true)
            {
                buttonSaveFileName.Content = Localisation.get("Button_SaveFileNameSave");
                textboxSaveFileName.IsReadOnly = false;
            }
            else
            {
                buttonSaveFileName.Content = Localisation.get("Button_SaveFileName");
                textboxSaveFileName.IsReadOnly = true;
                Config.change("game_run_file_name", textboxSaveFileName.Text);
            }
        }

        private void buttonGamePathClick(object sender, RoutedEventArgs e)
        {
            //懒狗微软
            var folder = new VistaFolderBrowserDialog();
            folder.Description = "Crusader Kings III";
            folder.UseDescriptionForTitle = true;
            if (folder.ShowDialog() == true)
            {
                string path = folder.SelectedPath;

                if (path.Substring(path.LastIndexOf('\\') + 1).ToLower() != @"crusader kings iii")
                {
                    MessageBox.Show(Localisation.get("error_not_find_game_path") + "\r\n" + "Crusader Kings III", Localisation.get("error_error"), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                textboxGamePath.Text = path;
                MainWindow.gamePath = path;
            }
        }

        private void buttonApplyAdaptionClick(object sender, RoutedEventArgs e)
        {
            AnalyzeGameDataFiles.applyAdaption();
        }
    }
}
