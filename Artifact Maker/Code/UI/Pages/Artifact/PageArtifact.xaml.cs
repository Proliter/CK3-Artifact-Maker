using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArtifactMaker
{
    /// <summary>
    /// Page_artifact.xaml 的交互逻辑
    /// </summary>
    /// 
    public partial class PageArtifact : Page, MainPage
    {
        private string language = "";
        private int dataVersion;
        private bool? advancedMode;

        private usercontrolPreview preview = new usercontrolPreview();

        private ObservableCollection<ViewItems> modifierItems = new ObservableCollection<ViewItems>();
        private ObservableCollection<ViewItems> improvementItems = new ObservableCollection<ViewItems>();

        private List<string> dataDefaultNameCategory;
        private List<string> dataDefaultDescriptionCategory;
        private List<string> dataRarity;
        private List<string> dataTypeCategory;
        private List<string> dataVisualCategory;
        private List<string> dataVisualSource;
        private List<string> dataTemplateCategory;
        private List<string> dataModifierCategory;
        private List<string> dataImprovementCategory;

        private List<List<string>> dataDefaultNameDetail = new List<List<string>>();
        private List<List<string>> dataDefaultDescriptionDetail = new List<List<string>>();
        private List<List<string>> dataTypeDetail = new List<List<string>>();
        private List<List<string>> dataVisualDetail = new List<List<string>>();
        private List<List<string>> dataTemplateDetail = new List<List<string>>();
        private List<List<string>> dataModifierDetail = new List<List<string>>();
        private List<List<string>> dataImprovementDetail = new List<List<string>>();

        private List<List<List<string>>> dataModifierValue = new List<List<List<string>>>();
        private List<List<List<string>>> dataImprovementValue = new List<List<List<string>>>();


        private List<string> localisationDefaultNameCategory = new List<string>();
        private List<string> localisationDefaultDescriptionCategory = new List<string>();
        private List<string> localisationRarity = new List<string>();
        private List<string> localisationTypeCategory = new List<string>();
        private List<string> localisationVisualCategory = new List<string>();
        private List<string> localisationVisualSource = new List<string>();
        private List<string> localisationTemplateCategory = new List<string>();
        private List<string> localisationModifierCategory = new List<string>();
        private List<string> localisationImprovementCategory = new List<string>();

        private List<List<string>> localisationDefaultNameDetail = new List<List<string>>();
        private List<List<string>> localisationDefaultDescriptionDetail = new List<List<string>>();
        private List<List<string>> localisationTypeDetail = new List<List<string>>();
        private List<List<string>> localisationVisualDetail = new List<List<string>>();
        private List<List<string>> localisationTemplateDetail = new List<List<string>>();
        private List<List<string>> localisationModifierDetail = new List<List<string>>();
        private List<List<string>> localisationImprovementDetail = new List<List<string>>();

        public PageArtifact()
        {
            InitializeComponent();

            dataDefaultNameCategory = Data.get("default_name_category");
            dataDefaultDescriptionCategory = Data.get("default_description_category");
            dataRarity = Data.get("rarity");
            dataTypeCategory = Data.get("type_category");
            dataVisualCategory = Data.get("visual_category");
            dataVisualSource = Data.get("visual_source_items");
            dataTemplateCategory = Data.get("template_category");
            dataModifierCategory = Data.get("modifier_category");
            dataImprovementCategory = Data.get("improvement_category");
            //detail
            Data.loadDataSecondLevel(dataDefaultNameCategory, dataDefaultNameDetail, "default_name_");
            Data.loadDataSecondLevel(dataDefaultDescriptionCategory, dataDefaultDescriptionDetail, "default_description_");
            Data.loadDataSecondLevel(dataTypeCategory, dataTypeDetail, "type_detail_");
            Data.loadDataSecondLevel(dataVisualCategory, dataVisualDetail, "visual_detail_");
            Data.loadDataSecondLevel(dataTemplateCategory, dataTemplateDetail);
            Data.loadDataSecondLevel(dataModifierCategory, dataModifierDetail, "modifier_detail_");
            Data.loadDataSecondLevel(dataImprovementCategory, dataImprovementDetail);
            //detail value
            for (int i = 0; i < dataModifierCategory.Count; i++)
            {
                List<List<string>> tempItem = new List<List<string>>();
                string prefix = "value_";

                for (int j = 0; j < dataModifierDetail[i].Count; j++)
                {
                    List<string> temp = Data.get(prefix + dataModifierDetail[i][j]);
                    tempItem.Add(temp);
                }

                dataModifierValue.Add(tempItem);
            }
            for (int i = 0; i < dataImprovementCategory.Count; i++)
            {
                List<List<string>> tempItem = new List<List<string>>();
                string prefix = "value_";

                for (int j = 0; j < dataImprovementDetail[i].Count; j++)
                {
                    List<string> temp = Data.get(prefix + dataImprovementDetail[i][j]);
                    tempItem.Add(temp);
                }

                dataImprovementValue.Add(tempItem);
            }
            dataVersion = 0;


            //number input limit
            DataObject.AddPastingHandler(textboxMaxDurability, textboxPasteLimitNumber);

            //auto size listview
            ((INotifyPropertyChanged)listviewModifierColumn_0).PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "ActualWidth")
                {
                    double temp = gridListModifier_0.ActualWidth + gridListModifier_1.ActualWidth - listviewModifierColumn_0.ActualWidth;
                    if (temp < 10)
                    {
                        temp = 10.0;
                        listviewModifierColumn_1.Width = temp;
                        listviewModifierColumn_0.Width = gridListModifier_0.ActualWidth + gridListModifier_1.ActualWidth - listviewModifierColumn_1.ActualWidth;
                    }
                    else
                    {
                        listviewModifierColumn_1.Width = temp;
                    }
                }
            };
            ((INotifyPropertyChanged)listviewImprovementColumn_0).PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "ActualWidth")
                {
                    double temp = gridListImprovement_0.ActualWidth + gridListImprovement_1.ActualWidth - listviewImprovementColumn_0.ActualWidth;
                    if (temp < 10)
                    {
                        temp = 10.0;
                        listviewImprovementColumn_1.Width = temp;
                        listviewImprovementColumn_0.Width = gridListImprovement_0.ActualWidth + gridListImprovement_1.ActualWidth - listviewImprovementColumn_1.ActualWidth;
                    }
                    else
                    {
                        listviewImprovementColumn_1.Width = temp;
                    }
                }
            };

            contentcontrolIcon.Content = preview;
        }


        public void applyLocalisation()
        {
            if (Localisation.getLanguage() != language)
            {
                language = Localisation.getLanguage();
                preview.applyLocalisation();

                Localisation.applyLocalisation(gridPageArtifact);

                labelModifierSelectedNoticeRefresh();
                labelImprovementSelectedNoticeRefresh();

                //default name
                Localisation.loadLocalisationFirstLevel(dataDefaultNameCategory, localisationDefaultNameCategory, "default_");
                Localisation.loadLocalisationSecondLevel(dataDefaultNameDetail, localisationDefaultNameDetail, "default_");
                //default description
                Localisation.loadLocalisationFirstLevel(dataDefaultDescriptionCategory, localisationDefaultDescriptionCategory, "default_");
                Localisation.loadLocalisationSecondLevel(dataDefaultDescriptionDetail, localisationDefaultDescriptionDetail, "default_");
                //rarity
                Localisation.loadLocalisationFirstLevel(dataRarity, localisationRarity, "rarity_");
                //type
                Localisation.loadLocalisationFirstLevel(dataTypeCategory, localisationTypeCategory, "type_");
                Localisation.loadLocalisationSecondLevel(dataTypeDetail, localisationTypeDetail, "type_");
                //visual
                Localisation.loadLocalisationFirstLevel(dataVisualCategory, localisationVisualCategory, "visual_");
                Localisation.loadLocalisationSecondLevel(dataVisualDetail, localisationVisualDetail, "visual_");
                //template
                Localisation.loadLocalisationFirstLevel(dataTemplateCategory, localisationTemplateCategory);
                Localisation.loadLocalisationSecondLevel(dataTemplateDetail, localisationTemplateDetail);
                //modifier
                Localisation.loadLocalisationFirstLevel(dataModifierCategory, localisationModifierCategory, "modifier_");
                Localisation.loadLocalisationSecondLevel(dataModifierDetail, localisationModifierDetail, "modifier_");
                //improvement
                Localisation.loadLocalisationFirstLevel(dataImprovementCategory, localisationImprovementCategory, "improvement_");
                Localisation.loadLocalisationSecondLevel(dataImprovementDetail, localisationImprovementDetail, "improvement_detail_");
                //visual source
                Localisation.loadLocalisationFirstLevel(dataVisualSource, localisationVisualSource, "visual_source_");

                //listview
                listviewModifierColumn_0.Header = Localisation.get("ListView_ModifierColumn_0");
                listviewModifierColumn_1.Header = Localisation.get("ListView_ModifierColumn_1");
                listviewImprovementColumn_0.Header = Localisation.get("ListView_ImprovementColumn_0");
                listviewImprovementColumn_1.Header = Localisation.get("ListView_ImprovementColumn_1");
            }

            buttonSaveToRunFolder.Content = Localisation.get("Button_SaveToRunFolder") + Config.get("game_run_file_name");
        }

        public void checkAdvancedMode(bool isAdvancedMode)
        {
            advancedMode = isAdvancedMode;

            if ((bool)advancedMode)
            {
                gridWealth.Visibility = Visibility.Visible;
                gridQuality.Visibility = Visibility.Visible;

                gridTemplate.Visibility = Visibility.Visible;
                labelTemplate.Visibility = Visibility.Visible;

                checkboxHistoricalArtifact.IsEnabled = false;
            }
            else
            {
                gridWealth.Visibility = Visibility.Hidden;
                gridQuality.Visibility = Visibility.Hidden;

                gridTemplate.Visibility = Visibility.Collapsed;
                labelTemplate.Visibility = Visibility.Collapsed;

                checkboxHistoricalArtifact.IsEnabled = true;
            }
        }

        public void refreshItemsSource()
        {
            //ComboBox store selection
            Stack<Tuple<ComboBox, int>> storeCombobox = new Stack<Tuple<ComboBox, int>>();

            //refresh data source if data changed
            if (dataVersion != Data.version)
            {
                refreshDataSource();
                dataVersion = Data.version;
            }

            //store
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxRarity, comboboxRarity.SelectedIndex));


            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxDefaultNameCategory, comboboxDefaultNameCategory.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxDefaultDescriptionCategory, comboboxDefaultDescriptionCategory.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxTypeCategory, comboboxTypeCategory.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxVisualCategory, comboboxVisualCategory.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxTemplateCategory, comboboxTemplateCategory.SelectedIndex));

            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxDefaultNameDetail, comboboxDefaultNameDetail.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxDefaultDescriptionDetail, comboboxDefaultDescriptionDetail.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxTypeDetail, comboboxTypeDetail.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxVisualDetail, comboboxVisualDetail.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxVisualSource, comboboxVisualSource.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxTemplateDetail, comboboxTemplateDetail.SelectedIndex));


            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxModifierValue, comboboxModifierValue.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxImprovementValue, comboboxImprovementValue.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxModifierDetail, comboboxModifierDetail.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxImprovementDetail, comboboxImprovementDetail.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxModifierCategory, comboboxModifierCategory.SelectedIndex));
            storeCombobox.Push(new Tuple<ComboBox, int>(comboboxImprovementCategory, comboboxImprovementCategory.SelectedIndex));


            //first level
            storeAndRefreshCombobox(comboboxRarity, localisationRarity);
            storeAndRefreshCombobox(comboboxDefaultNameCategory, localisationDefaultNameCategory);
            storeAndRefreshCombobox(comboboxDefaultDescriptionCategory, localisationDefaultDescriptionCategory);
            storeAndRefreshCombobox(comboboxTypeCategory, localisationTypeCategory);
            storeAndRefreshCombobox(comboboxVisualCategory, localisationVisualCategory);
            storeAndRefreshCombobox(comboboxVisualSource, localisationVisualSource);
            storeAndRefreshCombobox(comboboxTemplateCategory, localisationTemplateCategory);
            storeAndRefreshCombobox(comboboxModifierCategory, localisationModifierCategory);
            storeAndRefreshCombobox(comboboxImprovementCategory, localisationImprovementCategory);


            //restore
            foreach (var item in storeCombobox)
            {
                item.Item1.SelectedIndex = item.Item2;
            }

            //listview items
            listviewModifier.ItemsSource = modifierItems;
            listviewModifier.Items.Refresh();
            listviewImprovement.ItemsSource = improvementItems;
            listviewImprovement.Items.Refresh();

            return;

            static void storeAndRefreshCombobox(ComboBox comboBox, List<string> loc)
            {
                comboBox.ItemsSource = loc;
                comboBox.Items.Refresh();
            }
        }

        private void refreshDataSource()
        {
            applyDataSource();
            applyLocalisationSource();
        }

        private void applyDataSource()
        {
            dataDefaultNameCategory = Data.get("default_name_category");
            dataDefaultDescriptionCategory = Data.get("default_description_category");
            dataTypeCategory = Data.get("type_category");
            dataVisualCategory = Data.get("visual_category");
            dataTemplateCategory = Data.get("template_category");
            dataModifierCategory = Data.get("modifier_category");
            //detail
            dataDefaultNameDetail = new List<List<string>>();
            dataDefaultDescriptionDetail = new List<List<string>>();
            dataTypeDetail = new List<List<string>>();
            dataVisualDetail = new List<List<string>>();
            dataTemplateDetail = new List<List<string>>();
            dataModifierDetail = new List<List<string>>();
            Data.loadDataSecondLevel(dataDefaultNameCategory, dataDefaultNameDetail, "default_name_");
            Data.loadDataSecondLevel(dataDefaultDescriptionCategory, dataDefaultDescriptionDetail, "default_description_");
            Data.loadDataSecondLevel(dataTypeCategory, dataTypeDetail, "type_detail_");
            Data.loadDataSecondLevel(dataVisualCategory, dataVisualDetail, "visual_detail_");
            Data.loadDataSecondLevel(dataTemplateCategory, dataTemplateDetail);
            Data.loadDataSecondLevel(dataModifierCategory, dataModifierDetail, "modifier_detail_");
            //detail value
            dataModifierValue = new List<List<List<string>>>();
            for (int i = 0; i < dataModifierCategory.Count; i++)
            {
                List<List<string>> tempItem = new List<List<string>>();
                string prefix = "value_";

                for (int j = 0; j < dataModifierDetail[i].Count; j++)
                {
                    List<string> temp = Data.get(prefix + dataModifierDetail[i][j]);
                    tempItem.Add(temp);
                }

                dataModifierValue.Add(tempItem);
            }
        }

        private void applyLocalisationSource()
        {
            language = "_temp";
            applyLocalisation();
        }

        private void comboboxArtifactSelectionChanged(object sender, RoutedEventArgs e)
        {
            string name = ((ComboBox)sender).Name;

            switch (name)
            {
                case "comboboxVisualDetail":    //choose VisualSource is Hidden or not
                    if (comboboxVisualDetail.SelectedIndex != -1)
                    {
                        List<string> visualSourceAllowlist = Data.get("visual_source_allowlist");
                        string selectedVisual = dataVisualDetail[comboboxVisualCategory.SelectedIndex][comboboxVisualDetail.SelectedIndex];

                        if (visualSourceAllowlist.Contains(selectedVisual))
                        {
                            gridVisualSource.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            comboboxVisualSource.SelectedIndex = -1;
                            gridVisualSource.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        comboboxVisualSource.SelectedIndex = -1;
                        gridVisualSource.Visibility = Visibility.Collapsed;
                    }
                    break;
                case "comboboxDefaultNameCategory": //special
                case "comboboxDefaultDescriptionCategory":  //special
                case "comboboxTypeCategory":
                case "comboboxVisualCategory":
                case "comboboxTemplateCategory":
                case "comboboxModifierCategory":
                case "comboboxImprovementCategory":
                case "comboboxModifierDetail":
                case "comboboxImprovementDetail":
                    changeSecondComboboxItemsSource(name, sender);
                    break;
                default:
                    break;
            }

            refreshIconPreview();

            return;

            void changeSecondComboboxItemsSource(string name, object sender)
            {
                switch (name)
                {
                    case "comboboxDefaultNameCategory": //special
                        if (comboboxDefaultNameCategory.SelectedIndex > 0)
                        {
                            comboboxDefaultNameDetail.Visibility = Visibility.Visible;
                            textboxName.Visibility = Visibility.Hidden;
                            comboboxDefaultNameDetail.ItemsSource = localisationDefaultNameDetail[comboboxDefaultNameCategory.SelectedIndex];
                        }
                        else
                        {
                            comboboxDefaultNameDetail.Visibility = Visibility.Hidden;
                            textboxName.Visibility = Visibility.Visible;
                            comboboxDefaultNameDetail.ItemsSource = null;
                        }
                        break;
                    case "comboboxDefaultDescriptionCategory":  //special
                        if (comboboxDefaultDescriptionCategory.SelectedIndex > 0)
                        {
                            comboboxDefaultDescriptionDetail.Visibility = Visibility.Visible;
                            textboxDescription.IsEnabled = false;
                            comboboxDefaultDescriptionDetail.ItemsSource = localisationDefaultDescriptionDetail[comboboxDefaultDescriptionCategory.SelectedIndex];
                        }
                        else
                        {
                            comboboxDefaultDescriptionDetail.Visibility = Visibility.Hidden;
                            textboxDescription.IsEnabled = true;
                            comboboxDefaultDescriptionDetail.ItemsSource = null;
                        }
                        break;
                    case "comboboxTypeCategory":
                        if (comboboxTypeCategory.SelectedIndex != -1)
                        {
                            comboboxTypeDetail.ItemsSource = localisationTypeDetail[comboboxTypeCategory.SelectedIndex];
                        }
                        else
                        {
                            comboboxTypeDetail.ItemsSource = null;
                        }
                        break;
                    case "comboboxVisualCategory":
                        if (comboboxVisualCategory.SelectedIndex != -1)
                        {
                            comboboxVisualDetail.ItemsSource = localisationVisualDetail[comboboxVisualCategory.SelectedIndex];
                        }
                        else
                        {
                            comboboxVisualDetail.ItemsSource = null;
                        }
                        break;
                    case "comboboxTemplateCategory":
                        if (comboboxTemplateCategory.SelectedIndex != -1)
                        {
                            comboboxTemplateDetail.ItemsSource = localisationTemplateDetail[comboboxTemplateCategory.SelectedIndex];
                        }
                        else
                        {
                            comboboxTemplateDetail.ItemsSource = null;
                        }
                        break;
                    case "comboboxModifierCategory":
                        if (comboboxModifierCategory.SelectedIndex != -1)
                        {
                            comboboxModifierDetail.ItemsSource = localisationModifierDetail[comboboxModifierCategory.SelectedIndex];
                        }
                        else
                        {
                            comboboxModifierDetail.ItemsSource = null;
                        }
                        break;
                    case "comboboxImprovementCategory":
                        if (comboboxImprovementCategory.SelectedIndex != -1)
                        {
                            comboboxImprovementDetail.ItemsSource = localisationImprovementDetail[comboboxImprovementCategory.SelectedIndex];
                        }
                        else
                        {
                            comboboxImprovementDetail.ItemsSource = null;
                        }
                        break;
                    case "comboboxModifierDetail":
                        if (comboboxModifierDetail.SelectedIndex >= 0)
                        {
                            comboboxModifierValue.ItemsSource = dataModifierValue[comboboxModifierCategory.SelectedIndex][comboboxModifierDetail.SelectedIndex];
                        }
                        else
                        {
                            comboboxModifierValue.ItemsSource = null;
                        }
                        break;
                    case "comboboxImprovementDetail":
                        if (comboboxImprovementDetail.SelectedIndex >= 0)
                        {
                            comboboxImprovementValue.ItemsSource = dataImprovementValue[comboboxImprovementCategory.SelectedIndex][comboboxImprovementDetail.SelectedIndex];
                        }
                        else
                        {
                            comboboxImprovementValue.ItemsSource = null;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void textboxInputLimitNumber(object sender, TextCompositionEventArgs e)
        {
            string temp = ((TextBox)sender).Text + e.Text;

            if (temp == "0")
            {
                return;
            }

            Regex zeroRegex = new Regex(@"^0+$");//only 0
            if (zeroRegex.IsMatch(temp))
            {
                e.Handled = true;
                ((TextBox)sender).Text = "0";
                return;
            }

            Regex regex = new Regex(@"^[1-9]\d*$");
            if (!regex.IsMatch(temp))
            {
                e.Handled = true;
            }
        }

        private void textboxPasteLimitNumber(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true))
            {
                return;
            }

            string? temp = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            if (temp == null)
            {
                return;
            }

            Regex regex = new Regex(@"^[1-9]\d*$");

            if (!regex.IsMatch(temp))
            {
                Regex regex_1 = new Regex(@"^\d+$");
                if (String.IsNullOrEmpty(((TextBox)sender).Text) || !regex_1.IsMatch(temp))
                {
                    e.CancelCommand();
                }
            }
        }

        private void checkboxDefaultDurabilityCheckedChange(object sender, RoutedEventArgs e)
        {
            var isChecked = ((CheckBox)sender).IsChecked;

            if (isChecked == null || textboxMaxDurability == null)
            {
                return;
            }

            if ((bool)isChecked)
            {
                textboxMaxDurability.IsEnabled = false;
            }
            else
            {
                textboxMaxDurability.IsEnabled = true;
            }
        }

        private void buttonModifierAddClick(object sender, RoutedEventArgs e)
        {
            if (comboboxModifierDetail.SelectedIndex < 0 || comboboxModifierValue.SelectedIndex < 0)
            {
                return;
            }

            string key = dataModifierDetail[comboboxModifierCategory.SelectedIndex][comboboxModifierDetail.SelectedIndex];
            string num = dataModifierValue[comboboxModifierCategory.SelectedIndex][comboboxModifierDetail.SelectedIndex][comboboxModifierValue.SelectedIndex];

            modifierItems.Add(new ViewItems(key, num, comboboxModifierValue.SelectedIndex + 1, "modifier_"));
            labelModifierSelectedNoticeRefresh();
        }

        private void buttonImprovementAddClick(object sender, RoutedEventArgs e)
        {
            if (comboboxImprovementDetail.SelectedIndex < 0 || comboboxImprovementValue.SelectedIndex < 0)
            {
                return;
            }

            string key = dataImprovementDetail[comboboxImprovementCategory.SelectedIndex][comboboxImprovementDetail.SelectedIndex];
            string num = dataImprovementValue[comboboxImprovementCategory.SelectedIndex][comboboxImprovementDetail.SelectedIndex][comboboxImprovementValue.SelectedIndex];

            improvementItems.Add(new ViewItems(key, num, comboboxImprovementValue.SelectedIndex + 1, "improvement_detail_"));
            labelImprovementSelectedNoticeRefresh();
        }

        private void buttonModifierClearClick(object sender, RoutedEventArgs e)
        {
            modifierItems.Clear();
            labelModifierSelectedNoticeRefresh();
        }

        private void buttonModifierSelectedDeleteClick(object sender, RoutedEventArgs e)
        {
            var temp = listviewModifier.SelectedItems;

            while (temp.Count > 0)
            {
                ViewItems? deleteTarget = (ViewItems?)temp[0];
                if (deleteTarget != null)
                {
                    modifierItems.Remove(deleteTarget);
                }
            }

            labelModifierSelectedNoticeRefresh();
        }

        private void buttonImprovementClearClick(object sender, RoutedEventArgs e)
        {
            improvementItems.Clear();
            labelImprovementSelectedNoticeRefresh();
        }

        private void buttonImprovementSelectedDeleteClick(object sender, RoutedEventArgs e)
        {
            var temp = listviewImprovement.SelectedItems;

            while (temp.Count > 0)
            {
                ViewItems? deleteTarget = (ViewItems?)temp[0];
                if (deleteTarget != null)
                {
                    improvementItems.Remove(deleteTarget);
                }
            }

            labelImprovementSelectedNoticeRefresh();
        }

        private void listviewModifierSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            labelModifierSelectedNoticeRefresh();
        }

        private void listviewImprovementSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            labelImprovementSelectedNoticeRefresh();
        }

        private void labelModifierSelectedNoticeRefresh()
        {
            string res = Localisation.get("Label_ModifierSelectedNotice").Replace(@"[items.count]", modifierItems.Count.ToString()).Replace(@"[items.selected]", listviewModifier.SelectedItems.Count.ToString());
            labelModifierSelectedNotice.Content = res;
        }

        private void labelImprovementSelectedNoticeRefresh()
        {
            string res = Localisation.get("Label_ImprovementSelectedNotice").Replace(@"[items.count]", improvementItems.Count.ToString()).Replace(@"[items.selected]", listviewImprovement.SelectedItems.Count.ToString());
            labelImprovementSelectedNotice.Content = res;
        }

        private static bool checkAndCreateFolder(string checkPath, string folder)
        {
            if (Directory.Exists(checkPath))
            {
                Directory.CreateDirectory(checkPath + @"\" + folder);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void buttonOpenRunFolderClick(object sender, RoutedEventArgs e)
        {
            string path = Config.get("game_document_path");
            if (path.Contains("%userProfile%"))
            {
                path = Environment.ExpandEnvironmentVariables(path);
            }

            if (!checkAndCreateFolder(path, "run"))
            {
                MessageBox.Show(Localisation.get("MessageBox_not_find_game") + "\r\n" + path, Localisation.get("MessageBox_warnning"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            path += @"run";
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void buttonCopyRunCommandClick(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText("run " + Config.get("game_run_file_name"));
        }

        private void buttonSaveToRunFolderClick(object sender, RoutedEventArgs e)
        {
            string path = Config.get("game_document_path");
            if (path.Contains("%userProfile%"))
            {
                path = Environment.ExpandEnvironmentVariables(path);
            }

            if (!checkAndCreateFolder(path, "run"))
            {
                MessageBox.Show(Localisation.get("MessageBox_not_find_game") + "\r\n" + path, Localisation.get("MessageBox_warnning"), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            path += @"run\" + Config.get("game_run_file_name");
            string res = textboxOutput.Text;

            if (String.IsNullOrEmpty(res))
            {
                MessageBox.Show(Localisation.get("MessageBox_empty_info"), Localisation.get("MessageBox_warnning"), MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                using (StreamWriter runFile = new StreamWriter(path))
                {
                    runFile.WriteLine(res);
                }
            }
        }

        private void textboxOutputTextChanged(object sender, TextChangedEventArgs e)
        {
            int maxLength = int.Parse(Config.get("game_max_command_length"));
            if (textboxOutput.Text.Length > maxLength)
            {
                labelNotice.Visibility = Visibility.Visible;
            }
            else
            {
                labelNotice.Visibility = Visibility.Hidden;
            }
        }

        private void buttonCopyOutputClick(object sender, RoutedEventArgs e)
        {
            string temp = "effect " + textboxOutput.Text;

            if (temp.Length > 0)
            {
                Clipboard.SetText(temp);
            }
        }

        private void buttonClearClick(object sender, RoutedEventArgs e)
        {
            textboxOutput.Clear();
            textboxName.Clear();
            textboxQuality.Clear();
            textboxWealth.Clear();
            textboxMaxDurability.Clear();
            textboxDescription.Clear();

            modifierItems.Clear();
            improvementItems.Clear();

            checkboxDefaultDurability.IsChecked = true;
            checkboxInfiniteDurability.IsChecked = false;
            checkboxHistoricalArtifact.IsChecked = false;
            
            comboboxDefaultNameCategory.SelectedIndex = -1;
            comboboxDefaultDescriptionCategory.SelectedIndex = -1;
            comboboxRarity.SelectedIndex = -1;
            comboboxTypeCategory.SelectedIndex = -1;
            comboboxVisualCategory.SelectedIndex = -1;
            comboboxTemplateCategory.SelectedIndex = -1;
            comboboxModifierCategory.SelectedIndex = -1;
            comboboxImprovementCategory.SelectedIndex = -1;

            labelModifierSelectedNoticeRefresh();
            labelImprovementSelectedNoticeRefresh();
        }

        private void buttonGenerateClick(object sender, RoutedEventArgs e)
        {
            if (!checkAndWarnArtifact())
            {
                return;
            }

            if (advancedMode == null)
            {
                throw new Exception("Logic Error");
            }

            Artifact artifact = new Artifact();
            //name
            if (comboboxDefaultNameCategory.SelectedIndex > 0 && comboboxDefaultNameDetail.SelectedIndex >= 0)
            {
                List<string> target = Data.get("default_" + dataDefaultNameDetail[comboboxDefaultNameCategory.SelectedIndex][comboboxDefaultNameDetail.SelectedIndex] + "_name");
                artifact.name = target[0];

                for(int i = 1; i < target.Count; i++)
                {
                    artifact.scopes.Add(target[i]);
                }
            }
            else
            {
                artifact.name = '"' + textboxName.Text.Replace(@"\", @"\\").Replace("\"", "\\\"") + '"';
            }
            //description
            if (comboboxDefaultDescriptionCategory.SelectedIndex > 0 && comboboxDefaultDescriptionDetail.SelectedIndex >= 0)
            {
                List<string> target = Data.get("default_" + dataDefaultDescriptionDetail[comboboxDefaultDescriptionCategory.SelectedIndex][comboboxDefaultDescriptionDetail.SelectedIndex] + "_description");
                artifact.description = target[0];

                for (int i = 1; i < target.Count; i++)
                {
                    artifact.scopes.Add(target[i]);
                }
            }
            else
            {
                artifact.description = '"' + textboxDescription.Text.Replace(@"\", @"\\").Replace("\"", "\\\"") + '"';
            }


            artifact.rarity = dataRarity[comboboxRarity.SelectedIndex];
            artifact.type = dataTypeDetail[comboboxTypeCategory.SelectedIndex][comboboxTypeDetail.SelectedIndex];
            artifact.visuals = dataVisualDetail[comboboxVisualCategory.SelectedIndex][comboboxVisualDetail.SelectedIndex];
            if(comboboxVisualSource.SelectedIndex >= 0)
            {
                artifact.visualSource = Data.get("visual_source_item_" + dataVisualSource[comboboxVisualSource.SelectedIndex])[0];
            }

            //template
            if (advancedMode == true)
            {
                if (comboboxTemplateDetail.SelectedIndex >= 0)
                {
                    string sourceTemplate = dataTemplateDetail[comboboxTemplateCategory.SelectedIndex][comboboxTemplateDetail.SelectedIndex];
                    if (sourceTemplate.Contains('@'))//has variable
                    {
                        Regex regex = new Regex(@"(.+)@(.+)\|(.+)");
                        var result = regex.Match(sourceTemplate);

                        artifact.template = result.Groups[1].Value;
                        artifact.variables.Add(result.Groups[2].Value, result.Groups[3].Value);

                        //artifact.template = sourceTemplate.Substring(0, sourceTemplate.IndexOf('='));
                        //string sourceVariable = sourceTemplate.Substring(sourceTemplate.IndexOf('=') + 1);
                        //artifact.variables.Add(sourceVariable.Substring(0, sourceVariable.IndexOf('|')), sourceVariable.Substring(sourceVariable.IndexOf('|') + 1));
                    }
                    else
                    {
                        artifact.template = sourceTemplate;
                    }
                }
            }
            else
            {
                if (checkboxHistoricalArtifact.IsChecked == true && checkboxHistoricalArtifact.IsEnabled == true)
                {
                    artifact.template = Config.get("historical_artifact_template");
                }
            }
            //max durability
            if (checkboxDefaultDurability.IsChecked == true)
            {

            }
            else
            {
                if (!String.IsNullOrEmpty(textboxMaxDurability.Text))
                {
                    artifact.maxDurability = int.Parse(textboxMaxDurability.Text);
                }
            }
            //decaying
            if (checkboxInfiniteDurability.IsChecked == true)
            {
                artifact.decaying = false;
            }
            //wealth and quality
            if (advancedMode == true)
            {
                if (!String.IsNullOrEmpty(textboxWealth.Text))
                {
                    artifact.wealth = int.Parse(textboxWealth.Text);
                }
                if (!String.IsNullOrEmpty(textboxQuality.Text))
                {
                    artifact.quality = int.Parse(textboxQuality.Text);
                }
            }
            //modifiers
            foreach (var item in modifierItems)
            {
                artifact.modifiers.Add(new Tuple<string, int>(item.getOriginKey(), item.getLevel()));
            }
            //improvements
            foreach (var item in improvementItems)
            {
                artifact.improvements.Add(new Tuple<string, int>(item.getOriginKey(), item.getLevel()));
            }

            //go
            textboxOutput.Text = artifact.generator();
        }

        private bool checkAndWarnArtifact()
        {
            bool isOK = true;
            string warningMessage = "";

            if (comboboxRarity.SelectedIndex == -1)
            {
                isOK = false;
                warningMessage += Localisation.get("warning_message_rarity") + "\r\n";
            }
            if (comboboxTypeDetail.SelectedIndex == -1)
            {
                isOK = false;
                warningMessage += Localisation.get("warning_message_type") + "\r\n";
            }
            if (comboboxVisualDetail.SelectedIndex == -1)
            {
                isOK = false;
                warningMessage += Localisation.get("warning_message_visual") + "\r\n";
            }
            if (checkboxDefaultDurability.IsChecked == false && String.IsNullOrEmpty(textboxMaxDurability.Text))
            {
                isOK = false;
                warningMessage += Localisation.get("warning_message_max_durability") + "\r\n";
            }
            if (modifierItems.Count == 0)
            {
                isOK = false;
                warningMessage += Localisation.get("warning_message_modifier") + "\r\n";
            }

            if (!isOK)
            {
                MessageBox.Show(warningMessage, Localisation.get("MessageBox_warnning"), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return isOK;
        }

        private void buttonTemplateClearClick(object sender, RoutedEventArgs e)
        {
            comboboxTemplateCategory.SelectedIndex = -1;
        }

        private void buttonVisualSourceClick(object sender, RoutedEventArgs e)
        {
            comboboxVisualSource.SelectedIndex = -1;
        }

        private void checkboxHistoricalArtifactCheckedChange(object sender, RoutedEventArgs e)
        {
            refreshIconPreview();
        }

        private void refreshIconPreview()
        {
            bool isHistorical = false;
            string visual = "";

            if ((checkboxHistoricalArtifact.IsChecked == true && checkboxHistoricalArtifact.IsEnabled == true) || (comboboxTemplateDetail.SelectedIndex != -1 && advancedMode == true))
            {
                isHistorical = true;
            }

            if (comboboxVisualCategory.SelectedIndex != -1 && comboboxVisualDetail.SelectedIndex != -1)
            {
                visual = dataVisualDetail[comboboxVisualCategory.SelectedIndex][comboboxVisualDetail.SelectedIndex];
            }

            preview.refreshIcon(visual, comboboxRarity.SelectedIndex, isHistorical);

            if (preview.stackpanelIcon.Children.Count > 1)
            {
                Grid.SetColumnSpan(textboxDescription, 1);
                Grid.SetRowSpan(contentcontrolIcon, 3);
            }
            else
            {
                Grid.SetColumnSpan(textboxDescription, 2);
                Grid.SetRowSpan(contentcontrolIcon, 2);
            }
        }
    }

    public class ViewItems : INotifyPropertyChanged
    {
        string prefix;

        string _key = null!;
        string _num = null!;
        int level;

        public string key
        {
            get
            {
                return Localisation.get(prefix + _key);
            }
            set
            {
                _key = value;
                OnPropertyChanged("key");
            }
        }
        public string num
        {
            get
            {
                return _num;
            }
            set
            {
                _num = value;
                OnPropertyChanged("num");
            }
        }

        public ViewItems(string key, string num, int level, string prefix = "")
        {
            this.key = key;
            this.num = num;
            this.level = level;
            this.prefix = prefix;
        }

        public string getOriginKey()
        {
            return _key;
        }

        public int getLevel()
        {
            return level;
        }

        //notify
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(e));
            }
        }
    }
}
