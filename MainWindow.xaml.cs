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


namespace GMEPTitle24;

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
        options.AddArgument("headless");
        driver = new ChromeDriver(service, options);

        StatusText.Text = "Navigating to Site";
      
        int result = await Task.Run(int() =>
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
        int result =  await Task.Run(int() =>
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

        int result = await Task.Run(int() =>
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
    public async Task IndoorLighting()
    {
        StatusText.Text = "Navigating to Indoor Lighting";
        int result = await Task.Run(int() =>
        {
            try
            {
                IWebElement indoorLighting = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[text()='NRCC-LTI-E Indoor Lighting']")));
                indoorLighting.Click();
            }
            catch (WebDriverTimeoutException ex)
            {
                // Handle timeout exceptions
                Dispatcher.Invoke(() =>
                {
                    StatusText.Text = "Indoor Lighting Section not found. Please try again.";
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
        await FillOutLuminaires();
        await FillOutControls();
        await FillOutAllowances();
    }
    public async Task FillOutLuminaires()
    {
        StatusText.Text = "Filling Out Luminaires Section";
        int result =  await Task.Run(int() =>
        {
            try
            {
                IWebElement luminaires = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Luminaires']")));
                luminaires.Click();
                try
                {
                    WebDriverWait continueWait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                    IWebElement continueAnyway = continueWait.Until(driver =>
                    {
                        try
                        {
                            return driver.FindElement(By.XPath("//div[text()='Continue Anyway']"));
                        }
                        catch (NoSuchElementException)
                        {
                            return null;
                        }
                    });

                    if (continueAnyway != null)
                    {
                        continueAnyway.Click();
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    // If the "continue anyway" div is not found within the timeout, proceed
                    Debug.WriteLine("The 'continue anyway' div was not found. Proceeding...");
                }

                //Grabbing Container For All Lighting Entries
                IWebElement AddLuminaireButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Add Luminaire']")));

                IWebElement lightingContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_Table2_Row']")));
                var Lightings = lightingContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));

                //Removing all entries and adding new ones
                foreach (var lighting in Lightings)
                {
                    var delete = lighting.FindElement(By.CssSelector("div[class='mod_supportControl']"));
                    var deleteIcon = delete.FindElement(By.CssSelector("i"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteIcon);
                }

                foreach(var lighting in LightingList)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddLuminaireButton);

                    // Wait for the DOM to reflect the addition of the new luminaire
                    wait.Until(driver =>
                    {
                        var updatedLightings = lightingContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));
                        return updatedLightings.Count >= LightingList.IndexOf(lighting) + 1;
                    });
                }
                
                   
                Lightings = lightingContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));
                //Editing Boxes
                int row = 0;
                foreach (var lighting in Lightings)
                {
                    //iterating through text entries
                    var elements = lighting.FindElements(By.CssSelector("input[type='text']"));
                    foreach (var element in elements)
                    {
                        string attributeValue = element.GetAttribute("placeholder");
                        if (attributeValue != null && attributeValue.Contains("tag name", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].Tag);
                        }
                        if (attributeValue != null && attributeValue.Contains("luminaire description", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].Description);
                        }
                        if (attributeValue != null && attributeValue.Contains("watts per luminaire", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].Wattage);
                        }
                        if (attributeValue != null && attributeValue.Contains("volt-ampere rating", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].VoltAmpRating);
                        }
                        if (attributeValue != null && attributeValue.Contains("how many luminaires", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].LuminaireQty);
                        }
                        if (attributeValue != null && attributeValue.Contains("branch circuit", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].BranchCircuitVoltage);
                        }
                        if (attributeValue != null && attributeValue.Contains("combined ampacity", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].CombinedBreakerAmps);
                        }
                        if (attributeValue != null && attributeValue.Contains("maximum rated input wattage", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].MaxInputWattage);
                        }
                        if (attributeValue != null && attributeValue.Contains("are in conditioned space", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].ConditionedQty);
                        }
                        if (attributeValue != null && attributeValue.Contains("are in unconditioned space", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].UnconditionedQty);
                        }
                        if (attributeValue != null && attributeValue.Contains("linear feet", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].LinearFeet);
                        }
                    }
                    //Iterating Through dropdown entries
                    var dropdownElements = lighting.FindElements(By.CssSelector("div[class='selectWrapper']"));
                    foreach (var element in dropdownElements)
                    {
                        var textbox = element.FindElement(By.CssSelector("input"));
                        string placeholderValue = textbox.GetAttribute("placeholder");
                        if (placeholderValue != null && placeholderValue.Contains("luminaire type.", StringComparison.OrdinalIgnoreCase))
                        {
                            switch (LightingList[row].TypeId)
                            {
                                case 1:
                                    var individualLuminaire = element.FindElement(By.CssSelector("li[data-value='TypicalLuminaire'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", individualLuminaire);
                                    break;
                                case 2:
                                    var trackLighting = element.FindElement(By.CssSelector("li[data-value='TrackLighting'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", trackLighting);
                                    break;
                                case 3:
                                    var smallAperatureColor = element.FindElement(By.CssSelector("li[data-value='SmallAperatureColor'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", smallAperatureColor);
                                    break;
                            }
                        }
                        if (placeholderValue != null && placeholderValue.Contains("general lighting", StringComparison.OrdinalIgnoreCase))
                        {
                            switch (LightingList[row].IsDecorative)
                            {
                                case true:
                                    var AccentDecorativeLighting = element.FindElement(By.CssSelector("li[data-value='AccentDecorativeLighting'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AccentDecorativeLighting);
                                    break;
                                default:
                                    var GeneralLighting = element.FindElement(By.CssSelector("li[data-value='GeneralLighting'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", GeneralLighting);
                                    break;
                            }
                        }
                        if (placeholderValue != null && placeholderValue.Contains("Wattage Determined", StringComparison.OrdinalIgnoreCase))
                        {
                            switch (LightingList[row].WattageSourceId)
                            {
                                case 1:
                                    var ManufacturerSpec = element.FindElement(By.CssSelector("li[data-value='ManufacturerSpec'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", ManufacturerSpec);
                                    break;
                                case 2:
                                    var Default = element.FindElement(By.CssSelector("li[data-value='CEC_Default'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", Default);
                                    break;
                                case 3:
                                    var Other = element.FindElement(By.CssSelector("li[data-value='Other'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", Other);
                                    break;
                            }
                        }
                        if (placeholderValue != null && placeholderValue.Contains("Conditioned", StringComparison.OrdinalIgnoreCase))
                        {
                            var ConditionedBox = element.FindElement(By.CssSelector("div[data-name='Conditioned'"));
                            var UnconditionedBox = element.FindElement(By.CssSelector("div[data-name='Unconditioned'"));
                            var ConditionedBoxLabel = ConditionedBox.FindElement(By.CssSelector("label"));
                            var UnconditionedBoxLabel = UnconditionedBox.FindElement(By.CssSelector("label"));
                            switch (LightingList[row].ConditionedTypeId) {
                                case 1:
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", ConditionedBoxLabel);
                                    break;
                                case 2:
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", UnconditionedBoxLabel);
                                    break;
                                case 3:
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", ConditionedBoxLabel);
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", UnconditionedBoxLabel);
                                    break;
                            }
                        }
                        if (placeholderValue != null && placeholderValue.Contains("excluded from total", StringComparison.OrdinalIgnoreCase))
                        {
                            switch (LightingList[row].IsExcluded)
                            {
                                case true:
                                    var Yes = element.FindElement(By.CssSelector("li[data-value='Yes'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", Yes);
                                    break;
                                default:
                                    var No = element.FindElement(By.CssSelector("li[data-value='No'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", No);
                                    break;
                            }
                        }
                        if (placeholderValue != null && placeholderValue.Contains("Occupancy", StringComparison.OrdinalIgnoreCase))
                        {
                            switch (LightingList[row].OccupancyTypeId)
                            {
                                case 1:
                                    var mf = element.FindElement(By.CssSelector("li[data-value='OccupancyMF'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", mf);
                                    break;
                                case 2:
                                    var nonRes = element.FindElement(By.CssSelector("li[data-value='OccupancyNonres'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();",nonRes);
                                    break;
                                case 3:
                                    var na = element.FindElement(By.CssSelector("li[data-value='OccupancyNA'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", na);
                                    break;
                            }
                        }
                        if (placeholderValue != null && placeholderValue.Contains("determine compliance", StringComparison.OrdinalIgnoreCase))
                        {
                            switch (LightingList[row].ComplianceMethodId)
                            {
                                case 1:
                                    var curr = element.FindElement(By.CssSelector("li[data-value='TrackLightingPowerCalculation2'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", curr);
                                    break;
                                case 2:
                                    var installed = element.FindElement(By.CssSelector("li[data-value='TrackLightingPowerCalculation1'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", installed);
                                    break;
                                case 3:
                                    var overCurr = element.FindElement(By.CssSelector("li[data-value='TrackLightingPowerCalculation3'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", overCurr);
                                    break;
                                case 4:
                                    var supplied = element.FindElement(By.CssSelector("li[data-value='TrackLightingPowerCalculation4'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", supplied);
                                    break;
                            }
                        }
                    }
                    row++;
                }
                IWebElement SaveButton = driver.FindElement(By.XPath("//div[text()='Save']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", SaveButton);

                WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                wait2.Until(driver =>
                {
                    var formElement = driver.FindElement(By.Id("matForm"));
                    return formElement.GetAttribute("class").Contains("mod_submitting");
                });

                // Wait for the "mod_submitting" class to be removed
                wait2.Until(driver =>
                {
                    var formElement = driver.FindElement(By.Id("matForm"));
                    return !formElement.GetAttribute("class").Contains("mod_submitting");
                });



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
                    StatusText.Text = "An error occurred while navigation to luminaires.";
                    Loading.Visibility = Visibility.Collapsed;
                });
                Debug.WriteLine($"WebDriver Exception: {ex.Message}");
                return 0;
            }
            return 1;
        });
        if (result == 0)
        {
            Debug.WriteLine("Meow");
            return;
        }
    }
    public async Task FillOutControls()
    {
        StatusText.Text = "Filling Out Controls Section";
        int result = await Task.Run(int () =>
        {
            try
            {
                IWebElement controls = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Controls']")));
                controls.Click();
                try
                {
                    WebDriverWait continueWait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                    IWebElement continueAnyway = continueWait.Until(driver =>
                    {
                        try
                        {
                            return driver.FindElement(By.XPath("//div[text()='Continue Anyway']"));
                        }
                        catch (NoSuchElementException)
                        {
                            return null;
                        }
                    });

                    if (continueAnyway != null)
                    {
                        continueAnyway.Click();
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    // If the "continue anyway" div is not found within the timeout, proceed
                    Debug.WriteLine("The 'continue anyway' div was not found. Proceeding...");
                }

                //Grabbing Container For All Lighting Entries
                IWebElement AddAreaButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Add New Area']")));

                IWebElement areaContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_Table5_Row']")));
                var areas = areaContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));

                //Removing all entries and adding new ones
                foreach (var area in areas)
                {
                    var delete = area.FindElement(By.CssSelector("div[class='mod_supportControl']"));
                    var deleteIcon = delete.FindElement(By.CssSelector("i"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteIcon);
                }

                foreach (var area in ControlAreaList)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddAreaButton);
                    wait.Until(driver =>
                    {
                        var updatedAreas = areaContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));
                        return updatedAreas.Count >= ControlAreaList.IndexOf(area) + 1;
                    });
                }

                areas = areaContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));
                //Editing Boxes
                int row = 0;
                foreach (var area in areas)
                {
                    //iterating through text entries
                    var elements = area.FindElements(By.CssSelector("input[type='text']"));
                    foreach (var element in elements)
                    {
                        string attributeValue = element.GetAttribute("placeholder");
                        if (attributeValue != null && attributeValue.Contains("area description", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ControlAreaList[row].Description);
                        }
                    }

                    var dropdownElements = area.FindElements(By.CssSelector("div[class='selectWrapper']"));
                    foreach (var element in dropdownElements)
                    {
                        var textbox = element.FindElement(By.CssSelector("input"));
                        string placeholderValue = textbox.GetAttribute("placeholder");
                        if (placeholderValue != null && placeholderValue.Contains("primary function area", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li:not(.mod_disabled)"));
                            Debug.WriteLine("Choices Number: " + choices.Count.ToString());
                            var choice = choices[controlAreaList[row].PrimaryFunctionId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("area controls", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[controlAreaList[row].AreaControlTypeId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("multi-level controls", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[controlAreaList[row].MultilevelControlTypeId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("shut-off controls", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[controlAreaList[row].ShutoffControlTypeId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("primary/skylit daylighting controls", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[controlAreaList[row].PrimaryDaylightControlTypeId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("secondary daylighting controls", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[controlAreaList[row].SecondaryDaylightControlTypeId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("interlocked lighting systems", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            int result = controlAreaList[row].InterlockedSystems ? 0 : 1;
                            var choice = choices[result];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                    }
                    row++;
                }

                IWebElement SaveButton = driver.FindElement(By.XPath("//div[text()='Save']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", SaveButton);

                WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                wait2.Until(driver =>
                {
                    var formElement = driver.FindElement(By.Id("matForm"));
                    return formElement.GetAttribute("class").Contains("mod_submitting");
                });

                // Wait for the "mod_submitting" class to be removed
                wait2.Until(driver =>
                {
                    var formElement = driver.FindElement(By.Id("matForm"));
                    return !formElement.GetAttribute("class").Contains("mod_submitting");
                });
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
                    StatusText.Text = "An error occurred while navigation to luminaires.";
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
    }
    public async Task FillOutAllowances()
    {
        StatusText.Text = "Filling Out Allowances Section";
        int result = await Task.Run(int () =>
        {
            try
            {
                IWebElement allowances = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Allowances']")));
                allowances.Click();
                try
                {
                    WebDriverWait continueWait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));
                    IWebElement continueAnyway = continueWait.Until(driver =>
                    {
                        try
                        {
                            return driver.FindElement(By.XPath("//div[text()='Continue Anyway']"));
                        }
                        catch (NoSuchElementException)
                        {
                            return null;
                        }
                    });

                    if (continueAnyway != null)
                    {
                        continueAnyway.Click();
                    }
                }
                catch (WebDriverTimeoutException)
                {
                    // If the "continue anyway" div is not found within the timeout, proceed
                    Debug.WriteLine("The 'continue anyway' div was not found. Proceeding...");
                }

                //Grabbing Container For All Lighting Entries
                IWebElement AddAreaButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Add Area']")));

                IWebElement areaContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_Table6_Row']")));
                IWebElement areaChildrenContainer = areaContainer.FindElement(By.CssSelector("div[class='molecule_children']"));
                var areas = areaChildrenContainer.FindElements(By.CssSelector(":scope > div[class='mod_multiField']"));
                Debug.WriteLine("Areas count: " + areas.Count);

                //Removing all entries and adding new ones
                foreach (var area in areas)
                {
                    var delete = area.FindElement(By.CssSelector(":scope > div[class='mod_supportControl']"));
                    var deleteIcon = delete.FindElement(By.CssSelector("i"));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteIcon);
                }

                foreach (var area in ControlAreaList)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddAreaButton);
                    wait.Until(driver =>
                    {
                        var updatedAreas = areaChildrenContainer.FindElements(By.CssSelector(":scope > div[class='mod_multiField']"));
                        return updatedAreas.Count >= ControlAreaList.IndexOf(area) + 1;
                    });
                }

                areas = areaChildrenContainer.FindElements(By.CssSelector(":scope > div[class='mod_multiField']"));
                //Editing Boxes
                int row = 0;
                foreach (var area in areas)
                {
                    //iterating through text entries
                    var elements = area.FindElements(By.CssSelector("input[type='text']"));
                    foreach (var element in elements)
                    {
                        string attributeValue = element.GetAttribute("placeholder");
                        if (attributeValue != null && attributeValue.Contains("square footage", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ControlAreaList[row].SquareFootage);
                        }
                    }

                    var dropdownElements = area.FindElements(By.CssSelector("div[class='selectWrapper']"));
                    foreach (var element in dropdownElements)
                    {
                        var textbox = element.FindElement(By.CssSelector("input"));
                        string placeholderValue = textbox.GetAttribute("placeholder");
                        if (placeholderValue != null && placeholderValue.Contains("area entered in the controls section", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[0];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("choose the controls section area", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[row];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("conditioned or unconditioned", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            int result = ControlAreaList[row].Conditioned ? 0 : 1;
                            var choice = choices[result];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("additional power allowance", StringComparison.OrdinalIgnoreCase))
                        {
                            var PowerAdjustmentBox = element.FindElement(By.CssSelector("div[data-name='PowerAdjustmentFactor'"));
                            var NoAdditionalBox = element.FindElement(By.CssSelector("div[data-name='NoAdditionalAllowances'"));
                            var PowerAdjustmentLabel = PowerAdjustmentBox.FindElement(By.CssSelector("label"));
                            var NoAdditionalLabel = NoAdditionalBox.FindElement(By.CssSelector("label"));
                            switch (ControlAreaList[row].PowerAdjustmentId)
                            {
                                case 1:
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", PowerAdjustmentLabel);
                                    break;
                                case 2:
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", NoAdditionalLabel);
                                    break;
                                case 3:
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", PowerAdjustmentLabel);
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", NoAdditionalLabel);
                                    break;
                            }
                        }
                    }
                    row++;
                }

                IWebElement SaveButton = driver.FindElement(By.XPath("//div[text()='Save']"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", SaveButton);

                WebDriverWait wait2 = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
                wait2.Until(driver =>
                {
                    var formElement = driver.FindElement(By.Id("matForm"));
                    return formElement.GetAttribute("class").Contains("mod_submitting");
                });

                // Wait for the "mod_submitting" class to be removed
                wait2.Until(driver =>
                {
                    var formElement = driver.FindElement(By.Id("matForm"));
                    return !formElement.GetAttribute("class").Contains("mod_submitting");
                });
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
                    StatusText.Text = "An error occurred while navigating to luminaires.";
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
        StatusText.Text = "";
        Loading.Visibility = Visibility.Collapsed;
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
        Loading.Visibility = Visibility.Visible;
        StatusText.Text = "Downloading";
        VersionComboBox.SelectedValue = 0;
        ProjectIds = await db.GetProjectIds(ProjectNo);
        if (ProjectIds.Count == 0)
        {
            StatusText.Text = "Project Not Found";
            ControlAreaGrid.IsEnabled = false;
            Loading.Visibility = Visibility.Collapsed;
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
            LightingList = await db.GetLighting(newProjectId);
            ControlAreaList = await db.GetControlAreas(newProjectId);

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
}