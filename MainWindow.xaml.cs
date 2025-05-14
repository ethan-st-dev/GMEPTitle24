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
    public partial class MainWindow : Window
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
                            viewModel.StatusText = "Project Not Found";
                        });
                    }
                    else if (args.Length == 2)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            VersionComboBox.SelectedValue = viewModel.ProjectIds.Keys.First();
                            viewModel.ProjectLoaded = true;
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
                                    viewModel.ProjectLoaded = true;
                                });
                            }
                            else
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    VersionComboBox.SelectedValue = viewModel.ProjectIds.Keys.First();
                                    viewModel.ProjectLoaded = true;
                                });
                            }
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                VersionComboBox.SelectedValue = viewModel.ProjectIds.Keys.First();
                                viewModel.ProjectLoaded = true;
                            });
                        }
                    }
                }
                );
            }
        }
        
        

        /*private async void Export_Click(object sender, RoutedEventArgs e)
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
        }*/
    }
}