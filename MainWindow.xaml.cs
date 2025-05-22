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
        Indoor indoor;
        Outdoor outdoor;
        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();
            string projectNo = string.Empty;
            string projectVersion = string.Empty;

            InitializeComponent();
            DataContext = viewModel;

            //setting indoor
            indoor = new Indoor(viewModel);
            IndoorTab.Content = indoor;

            //setting outdoor
            outdoor = new Outdoor(viewModel);
            OutdoorTab.Content = outdoor;   

            if (args.Length > 1)
            {
                viewModel.ProjectNo = args[1];
                viewModel.SaveProjectNo = args[1];
                Task.Run(async () =>
                {
                    viewModel.ProjectLoading = true;
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
                        if (args.Length == 4)
                        {
                            if (int.TryParse(args[3], out int indexKey))
                            {
                                if (indexKey == 0 || indexKey == 1)
                                {
                                    LightingTabs.SelectedIndex = indexKey;
                                }
                            }
                        }
                    }
                   
                    viewModel.ProjectLoading = false;
                }
                );
            }
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            if (indoor.viewModel.scopeData != null && VersionComboBox.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                viewModel.ProjectLoading = true;
                viewModel.StatusText = "Saving";
                await indoor.viewModel.SaveObjects(selectedPair.Value);
                await outdoor.viewModel.SaveObjects(selectedPair.Value);
                bool result = await viewModel.ActivateSelenium();
                if (result)
                {
                    if (LightingTabs.SelectedIndex == 0)
                    {
                        result = await indoor.viewModel.IndoorLighting();
                        if (result)
                        {
                            await viewModel.LaunchWindow();
                        }
                    }
                    else if (LightingTabs.SelectedIndex == 1)
                    {
                        result = await outdoor.viewModel.OutdoorLighting();
                        if (result)
                        {
                            await viewModel.LaunchWindow();
                        }
                    }
                }
                viewModel.StatusText = String.Empty;
                viewModel.ProjectLoading = false;
            }
        }
        private async void Download_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ProjectLoading = true;
            viewModel.StatusText = "Downloading";
            VersionComboBox.SelectedValue = 0;
            viewModel.ProjectIds = await viewModel.db.GetProjectIds(viewModel.ProjectNo);
            if (viewModel.ProjectIds.Count == 0)
            {
                viewModel.StatusText = "Project Not Found";
                viewModel.ProjectLoaded = false;
                viewModel.ProjectLoading = false;
                indoor.viewModel.ClearObjects();
                outdoor.viewModel.ClearObjects();
                return;
            }
            viewModel.ProjectLoaded = true;
            VersionComboBox.SelectedValue = viewModel.ProjectIds.Keys.First();
            viewModel.StatusText = "";
            viewModel.ProjectLoading = false;
            viewModel.SaveProjectNo = viewModel.ProjectNo;

        }
        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (indoor.viewModel.scopeData != null && VersionComboBox.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                viewModel.ProjectLoading = true;
                viewModel.StatusText = "Saving";
                await indoor.viewModel.SaveObjects(selectedPair.Value);
                await outdoor.viewModel.SaveObjects(selectedPair.Value);
                viewModel.StatusText = String.Empty;
                viewModel.ProjectLoading = false;
            }
        }
        private async void Version_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VersionComboBox.SelectedItem is KeyValuePair<int, string> selectedPair)
            {
                //Electrical Tab
                await indoor.viewModel.InitializeObjects(selectedPair.Value);
                await outdoor.viewModel.InitializeObjects(selectedPair.Value);
                viewModel.ProjectLoaded = true;
            }
        }
    }
}