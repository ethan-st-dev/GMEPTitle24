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
        ChromeOptions options;
        IWebDriver driver;
        WebDriverWait wait;
        public Dictionary<int, string> projectIds;

        public Database db = new Database();
        public Dictionary<int, string> ProjectIds
        {
            get { return projectIds; }
            set
            {
                if (projectIds != value)
                {
                    projectIds = value;
                    OnPropertyChanged(nameof(ProjectIds));
                }
            }
        }
        public string selectedProjectId;
        public string SelectedProjectId
        {
            get { return selectedProjectId; }
            set
            {
                selectedProjectId = value;
                OnPropertyChanged(nameof(SelectedProjectId));
            }
        }
        public string projectNo = string.Empty;
        public string ProjectNo
        {
            get { return projectNo; }
            set
            {
                projectNo = value;
                OnPropertyChanged(nameof(ProjectNo));
            }
        }
        public string saveProjectNo = string.Empty;
        public string SaveProjectNo
        {
            get { return saveProjectNo; }
            set
            {
                saveProjectNo = value;
                OnPropertyChanged(nameof(SaveProjectNo));
            }
        }
        public int ProjectVersion { get; set; }

        public Scope scopeData;
        public Scope ScopeData
        {
            get { return scopeData; }
            set
            {
                if (scopeData != value)
                {
                    scopeData = value;
                    OnPropertyChanged(nameof(ScopeData));
                }
            }
        }

        public ObservableCollection<Lighting> lightingList = new ObservableCollection<Lighting>();
        public ObservableCollection<Lighting> LightingList
        {
            get { return lightingList; }
            set
            {
                if (lightingList != value)
                {
                    lightingList = value;
                    OnPropertyChanged(nameof(LightingList));
                }
            }
        }
        public ObservableCollection<ControlArea> controlAreaList = new ObservableCollection<ControlArea>();
        public ObservableCollection<ControlArea> ControlAreaList
        {
            get { return controlAreaList; }
            set
            {
                if (controlAreaList != value)
                {
                    controlAreaList = value;
                    OnPropertyChanged(nameof(ControlAreaList));
                }
            }
        }
        public Dictionary<int, string> Buildings { get; set; } = new Dictionary<int, string>
        {
            {1, "Audience Seating Area" },
            {2, "Auditorium Area" },
            {3, "Assembly Building" },
            {4, "Auto Repair Area" },
            {5, "Barber/Beauty Salon/Spa Area"},
            {6, "Civic Meeting Place Area" },
            {7, "Classroom, Lecture, or Training Vocational Area" },
            {8, "Commercial Industrial Storage Area" },
            {9, "Commercial Industrial Storage Shipping Area" },
            {10, "Commercial Industrial Warehouse" },
            {11, "Concourse and Atria Area" },
            {12, "Conference, Multipurpose and Meeting Area"},
            {13, "Convention, Conference, Multipurpose and Meeting Center Areas"},
            {14, "Copy Room"},
            {15, "Corridor"},
            {16, "Corridor (Low Vision)"},
            {17, "Dining Area - Bar/Fine"},
            {18, "Dining Area - Family"},
            {19, "Dining Area - Fast Food"},
            {20, "Dining Area (Low Vision)"},
            {21, "Electrical Mechanical Telephone Room"},
            {22, "Exercise Center Gymnasium Area"},
            {23, "Exhibit Area - Museum"},
            {24, "Financial Institution Building" },
            {25, "Financial Transaction Area"},
            {26, "General Commercial Industrial Work Area High Bay"},
            {27, "General Commercial Industrial Work Area Low Bay"},
            {28, "General Commercial Industrial Work Area Precision"},
            {29, "Grocery Sales Area"},
            {30, "Grocery Store Building" },
            {31, "Gymnasium Building" },
            {32, "Healthcare / Assisted Living Nurse Station" },
            {33, "Healthcare / Assisted Living Physical Therapy" },
            {34, "Hospital Building" },
            {35, "Hospital Exam/Treatment Area"},
            {36, "Hospital - Imaging Area"},
            {37, "Hospital - Medical Supply Area"},
            {38, "Hospital - Nursery"},
            {39, "Hospital - Nurse Station"},
            {40, "Hospital - Operating Room"},
            {41, "Hospital - Patient Room"},
            {42, "Hospital - Physical Therapy Area"},
            {43, "Hospital - Recovery Area"},
            {44, "Hotel Function Area"},
            {45, "Kitchen/ Food Preparation Area"},
            {46, "Scientific Laboratory"},
            {47, "Laundry Area"},
            {48, "Library Building" },
            {49, "Library - Reading Area"},
            {50, "Library - Stacks"},
            {51, "Locker/Dressing Room"},
            {52, "Lounge Area"},
            {53, "Low Vision Lounge/Waiting Area"},
            {54, "Main Entry Lobby (Low Vision)"},
            {55, "Multipurpose Room (Low Vision)"},
            {56, "Main Entry Lobby"},
            {57, "Manufacturing Facility Building" },
            {58, "Medical And Clinical Care Area"},
            {59, "Motion Picture Building" },
            {60, "Museum Building" },
            {61, "Museum Restoration Area"},
            {62, "Office Building" },
            {63, "Office ≤250 square feet"},
            {64, "Office > 250 square feet"},
            {65, "Parking Garage Building" },
            {66, "Parking Garage - Daylight Adaptation Zone Area" },
            {67, "Parking Garage - Parking Area & Ramps" },
            {68, "Performing Arts Theater Building" },
            {69, "Pharmacy Area" },
            {70, "Religious Facility Building" },
            {71, "Religious Worship Area" },
            {72, "Religious Worship Area (Low Vision"},
            {73, "Restaurant Building" },
            {74, "Restroom" },
            {75, "Restroom (Low Vision)" },
            {76, "Retail Store" },
            {77, "Retail Merchandise Sales Wholesale Showroom" },
            {78, "Retail Fitting Room" },
            {79, "School Building" },
            {80, "Sports Arena Building" },
            {81, "Sports Arena Class I" },
            {82, "Sports Arena Class II" },
            {83, "Sports Arena Class III" },
            {84, "Sports Arena Class IV" },
            {85, "Stairwell" },
            {86, "Stairwell (Low Vision)" },
            {87, "Storage - MF common areas" },
            {88, "Theater - Motion Picture Area" },
            {89, "Theater - Performance Area" },
            {90, "Transportation - Concourse and Babbage Area" },
            {91, "Transportation - Ticketing Area" },
            {92, "Video Conferencing Studio Area" },
            {93, "All Other Buildings" },
            {94, "All Other Space Types" }
        };

        public Dictionary<int, string> filteredBuildings;
        public Dictionary<int, string> FilteredBuildings
        {
            get { return filteredBuildings; }
            set
            {
                if (filteredBuildings != value)
                {
                    filteredBuildings = value;
                    OnPropertyChanged(nameof(FilteredBuildings));
                }
            }
        }

        public MainWindow()
        {
            string[] args = Environment.GetCommandLineArgs();
            string projectNo = string.Empty;
            string projectVersion = string.Empty;

            InitializeComponent();
            DataContext = this;

            if (args.Length > 1)
            {
                ProjectNo = args[1];
                SaveProjectNo = args[1];
                Task.Run(async () =>
                {
                    ProjectIds = await db.GetProjectIds(ProjectNo);
                    if (ProjectIds.Count == 0)
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
                            VersionComboBox.SelectedValue = ProjectIds.Keys.First();
                            ControlAreaGrid.IsEnabled = true;
                        });
                    }
                    else if (args.Length > 2)
                    {
                        if (int.TryParse(args[2], out int versionKey))
                        {
                            if (ProjectIds.ContainsKey(versionKey))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    VersionComboBox.SelectedValue = versionKey;
                                    ControlAreaGrid.IsEnabled = true;
                                });
                            }
                            else
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    VersionComboBox.SelectedValue = ProjectIds.Keys.First();
                                    ControlAreaGrid.IsEnabled = true;
                                });
                            }
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                VersionComboBox.SelectedValue = ProjectIds.Keys.First();
                                ControlAreaGrid.IsEnabled = true;
                            });
                        }
                    }
                }
                );
            }
        }
        public async Task ActivateSelenium()
        {
            var service = ChromeDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;

            options = new ChromeOptions();
            options.AddArgument("headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--no-sandbox");
            driver = new ChromeDriver(service, options);

            StatusText.Text = "Navigating to Site";

            int result = await Task.Run(int () =>
            {
                try
                {
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    driver.Navigate().GoToUrl("https://energycodeace.com/");
                }
                catch (WebDriverTimeoutException ex)
                {
                    // Handle timeout exceptions
                    Dispatcher.Invoke(() =>
                    {
                        StatusText.Text = "Navigation timed out. Please try again.";
                        Loading.Visibility = Visibility.Collapsed;
                    });
                    Debug.WriteLine($"Timeout Exception: {ex.Message}");
                    return 0;
                }
                catch (WebDriverException ex)
                {
                    // Handle general WebDriver exceptions
                    Dispatcher.Invoke(() =>
                    {
                        StatusText.Text = "An error occurred while navigating to the site.";
                        Loading.Visibility = Visibility.Collapsed;

                    });
                    Debug.WriteLine($"WebDriver Exception: {ex.Message}");
                    return 0;
                }
                return 1;
            });
            if (result == 0)
            {
                return;
            }
            await Login();

            //Quitting and launching new window
            var url = driver.Url;
            driver.Quit();

            StatusText.Text = "Launching Window.";
            Loading.Visibility = Visibility.Visible;

            await Task.Run(() =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Ensures the default browser is used
                });
            });
            StatusText.Text = "";
            Loading.Visibility = Visibility.Collapsed;
        }

        public async Task Login()
        {
            StatusText.Text = "Logging In";
            int result = await Task.Run(int () =>
            {
                try
                {
                    IWebElement signInButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-action='signIn']")));
                    if (signInButton.GetAttribute("data-name") == "Sign In")
                    {
                        signInButton.Click();

                        // Wait for the email field to be visible
                        IWebElement emailBox = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='email']")));

                        // Wait for the password field to be visible
                        IWebElement passwordBox = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("input[id='password']")));

                        // Wait for the second "Sign In" button to be clickable
                        IWebElement signInButton2 = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("input[value='Sign In']")));

                        // Enter credentials and click the second "Sign In" button
                        emailBox.SendKeys("stevengr@gmepe.com");
                        passwordBox.SendKeys("eeacf@stSea49");
                        signInButton2.Click();
                    }
                }
                catch (WebDriverTimeoutException ex)
                {
                    // Handle timeout exceptions
                    Dispatcher.Invoke(() =>
                    {
                        StatusText.Text = "Navigation timed out. Please try again.";
                        Loading.Visibility = Visibility.Collapsed;
                    });
                    Debug.WriteLine($"Timeout Exception: {ex.Message}");
                    return 0;
                }
                catch (WebDriverException ex)
                {
                    // Handle general WebDriver exceptions
                    Dispatcher.Invoke(() =>
                    {
                        StatusText.Text = "An error occurred while logging in.";
                        Loading.Visibility = Visibility.Collapsed;
                    });
                    Debug.WriteLine($"WebDriver Exception: {ex.Message}");
                    return 0;
                }
                return 1;
            });
            if (result == 0)
            {
                return;
            }
            await GoToProject(SaveProjectNo);
        }
        public async Task GoToProject(string projectNo)
        {
            StatusText.Text = "Navigating To Project";

            int result = await Task.Run(int () =>
            {
                try
                {
                    IWebElement hoverElement = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("a[data-live_click_handler='toggleProfileMenu'")));

                    // Perform the hover action
                    Actions actions = new Actions(driver);
                    actions.MoveToElement(hoverElement).Perform();

                    IWebElement projectButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("a[data-name='My Account | My Projects']")));
                    projectButton.Click();

                    IWebElement projectElement = wait.Until(driver =>
                    {
                        var elements = driver.FindElements(By.CssSelector("div[data-filterable-data]"));
                        foreach (var element in elements)
                        {
                            string attributeValue = element.GetAttribute("data-filterable-data");
                            if (attributeValue != null && attributeValue.Contains(projectNo, StringComparison.OrdinalIgnoreCase))
                            {
                                return element; // Return the matching element
                            }
                        }
                        return null; // Return null if no matching element is found
                    });
                    if (projectElement != null)
                    {
                        projectElement.Click();
                    }
                }
                catch (WebDriverTimeoutException ex)
                {
                    // Handle timeout exceptions
                    Dispatcher.Invoke(() =>
                    {
                        StatusText.Text = "Project not loaded or found. Please create a project starting with the project number " + projectNo.ToString() + ".";
                        Loading.Visibility = Visibility.Collapsed;
                    });
                    Debug.WriteLine($"Timeout Exception: {ex.Message}");
                    return 0;
                }
                catch (WebDriverException ex)
                {
                    // Handle general WebDriver exceptions
                    Dispatcher.Invoke(() =>
                    {
                        StatusText.Text = "An error occurred while navigating to project.";
                        Loading.Visibility = Visibility.Collapsed;
                    });
                    Debug.WriteLine($"WebDriver Exception: {ex.Message}");
                    return 0;
                }
                return 1;
            });
            if (result == 0)
            {
                return;
            }
            await IndoorLighting();

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