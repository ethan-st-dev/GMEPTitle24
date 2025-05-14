using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Interactions;
using System.Diagnostics;
using OfficeOpenXml;
using System.IO;
using Path = System.IO.Path;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls.Primitives;


namespace GMEPTitle24
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        MainViewModel viewModel = new MainViewModel();
        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();
            string projectNo = string.Empty;
            string projectVersion = string.Empty;

            InitializeComponent();
            DataContext = viewModel;

            if (args.Length > 1)
            {
                viewModel.ProjectNo = args[1];
                viewModel.SaveProjectNo = args[1];
                Task.Run(async () =>
                {
                    viewModel.ProjectIds = await viewModel.db.GetProjectIds(viewModel.ProjectNo);
                    if (viewModel.ProjectIds.Count == 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            StatusText.Text = "Project Not Found";
                        });
                    }
                    else if (args.Length == 2)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            VersionComboBox.SelectedValue =  viewModel.ProjectIds.Keys.First();
                            //ControlAreaGrid.IsEnabled = true;
                            viewModel.IsProjectLoaded = true;
                        });
                    }
                    else if (args.Length > 2)
                    {
                        if (int.TryParse(args[2], out int versionKey))
                        {
                            if (viewModel.ProjectIds.ContainsKey(versionKey))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    VersionComboBox.SelectedValue = versionKey;
                                    //ControlAreaGrid.IsEnabled = true;
                                    viewModel.IsProjectLoaded = true;
                                });
                            }
                            else
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    VersionComboBox.SelectedValue = viewModel.ProjectIds.Keys.First();
                                    //ControlAreaGrid.IsEnabled = true;
                                    viewModel.IsProjectLoaded = true;
                                });
                            }
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                VersionComboBox.SelectedValue = viewModel.ProjectIds.Keys.First();
                                //ControlAreaGrid.IsEnabled = true;
                                viewModel.IsProjectLoaded = true;
                            });
                        }
                    }
                }
                );
            }
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            if (LightingList.Count > 0 && VersionComboBox.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                Loading.Visibility = Visibility.Visible;
                StatusText.Text = "Saving";
                await db.UpdateLuminaires(LightingList);
                await db.UpdateControlAreas(ControlAreaList, selectedPair.Value);
                await ActivateSelenium();
            }
        }
        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            LightingList.Clear();
            ControlAreaList.Clear();
            ScopeData = null;
            Loading.Visibility = Visibility.Visible;
            StatusText.Text = "Downloading";
            VersionComboBox.SelectedValue = 0;
            ProjectIds = await db.GetProjectIds(ProjectNo);
            if (ProjectIds.Count == 0)
            {
                StatusText.Text = "Project Not Found";
                ControlAreaGrid.IsEnabled = false;
                Loading.Visibility = Visibility.Collapsed;
                ProjectCover.Visibility = Visibility.Visible;
                return;
            }
            ControlAreaGrid.IsEnabled = true;
            VersionComboBox.SelectedValue = ProjectIds.Keys.First();
            StatusText.Text = String.Empty;
            Loading.Visibility = Visibility.Collapsed;
            SaveProjectNo = ProjectNo;

        }
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (LightingList.Count > 0 && VersionComboBox.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                Loading.Visibility = Visibility.Visible;
                StatusText.Text = "Saving";
                await db.UpdateLuminaires(LightingList);
                await db.UpdateControlAreas(ControlAreaList, selectedPair.Value);
                await db.UpdateScope(ScopeData, selectedPair.Value);
                StatusText.Text = String.Empty;
                Loading.Visibility = Visibility.Collapsed;
            }
        }
        private async void Version_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VersionComboBox.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                //Electrical Tab
                string newProjectId = selectedPair.Value;
                ScopeData = await db.GetScope(newProjectId);
                ScopeData.PropertyChanged += ScopeData_PropertyChanged;
                FilterBuildings();
                LightingList = await db.GetLighting(newProjectId);
                ControlAreaList = await db.GetControlAreas(newProjectId);
                ProjectCover.Visibility = Visibility.Collapsed;
            }
        }
        public void OptionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OptionsGrid.SelectedItem is Lighting selectedLighting)
            {
                AdditionalOptionsGrid.SelectedItem = selectedLighting;
                AdditionalOptionsGrid.ScrollIntoView(selectedLighting);
            }
        }

        public void AdditionalOptionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AdditionalOptionsGrid.SelectedItem is Lighting selectedLighting)
            {
                OptionsGrid.SelectedItem = selectedLighting;
                OptionsGrid.ScrollIntoView(selectedLighting);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ScopeData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Scope.SystemFlag))
            {
                if (!ScopeData.SystemFlag)
                {
                    Reset_RowHeight();
                }
            }
            if (e.PropertyName == nameof(Scope.AlteredSystem))
            {
                if (!ScopeData.AlteredSystem)
                {
                    Reset_ColumnWidth();
                }
            }
            if (e.PropertyName == nameof(Scope.CompletePrimaryFunctionList))
            {
                FilterBuildings();
                ResetPrimaryFunctionIds();
            }
        }
        private void Reset_RowHeight()
        {
            Row1.ClearValue(RowDefinition.HeightProperty);
            Row3.ClearValue(RowDefinition.HeightProperty);
        }
        private void Reset_ColumnWidth()
        {
            Column1.ClearValue(ColumnDefinition.WidthProperty);
            Column3.ClearValue(ColumnDefinition.WidthProperty);
        }
        public void FilterBuildings()
        {
            var keysToInclude = new HashSet<int> { 3, 24, 30, 31, 34, 48, 57, 59, 60, 62, 65, 68, 70, 73, 76, 79, 80, 93 };
            if (!ScopeData.CompletePrimaryFunctionList)
            {
                // Include only the specified keys
                FilteredBuildings = Buildings
                    .Where(kvp => keysToInclude.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            else
            {
                // Exclude the specified keys
                FilteredBuildings = Buildings
                    .Where(kvp => !keysToInclude.Contains(kvp.Key))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }
        public void ResetPrimaryFunctionIds()
        {
            foreach (var elem in controlAreaList)
            {
                if (!ScopeData.CompletePrimaryFunctionList)
                {
                    elem.PrimaryFunctionId = 93;
                }
                else
                {
                    elem.PrimaryFunctionId = 94;
                }
            }
        }
    }
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore() => new BindingProxy();

        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}