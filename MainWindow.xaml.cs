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
                            });
                        }
                        else
                        {
                            Dispatcher.Invoke(() =>
                            {
                                StatusText.Text = "Version Not Found";
                            });
                        }
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            StatusText.Text = "Invalid Version Argument";
                        });
                    }
                }
            }
            );
        }
    }
    public async Task ActivateSelenium()
    {
        options = new ChromeOptions();
        //options.AddArgument("headless");
        driver = new ChromeDriver(options);

        StatusText.Text = "Navigating to Site";
        await Task.Run(() =>
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            driver.Navigate().GoToUrl("https://energycodeace.com/");
        });
        await Login();

        //Quitting Program
        //driver.Quit();

    }

    public async Task Login()
    {
        StatusText.Text = "Logging In";
        await Task.Run(() =>
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
        });
        await GoToProject(SaveProjectNo);
    }
    public async Task GoToProject(string projectNo)
    {
        StatusText.Text = "Navigating To Project";

        await Task.Run(() =>
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
        });
        await IndoorLighting();

    }
    public async Task IndoorLighting()
    {
        StatusText.Text = "Navigating to Indoor Lighting";
        await Task.Run(() =>
        {
            IWebElement indoorLighting = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[text()='NRCC-LTI-E Indoor Lighting']")));
            indoorLighting.Click();
        });
        await FillOutLuminaires();
    }
    public async Task FillOutLuminaires()
    {
        StatusText.Text = "Filling Out Luminaires Section";
        await Task.Run(() =>
        {
            IWebElement luminaires = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Luminaires']")));
            luminaires.Click();

            //Grabbing Container For All Lighting Entries
            IWebElement AddLuminaireButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Add Luminaire']")));

            IWebElement lightingContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_Table2_Row']")));
            var Lightings =  lightingContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));

            int rowCount = LightingList.Count;

            int lightingCount = Lightings.Count;

            //Adjusting Box Count
            if (lightingCount < rowCount)
            {    //Adding Boxes
                while (lightingCount < rowCount)
                {
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddLuminaireButton);
                    lightingCount++;
                }
            }
            else if (lightingCount > rowCount)
            {
                //Removing Boxes
                foreach (var lighting in Lightings)
                {
                    if (lightingCount > rowCount)
                    {
                        var delete = lighting.FindElement(By.CssSelector("div[class='mod_supportControl']"));
                        var deleteIcon = delete.FindElement(By.CssSelector("i"));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteIcon);
                        lightingCount--;
                    }
                }
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
                    if (attributeValue != null && attributeValue.Contains("Watts per Luminaire", StringComparison.OrdinalIgnoreCase))
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, LightingList[row].Wattage);
                    }
                }
                //Iterating Through dropdown entries
                var dropdownElements = lighting.FindElements(By.CssSelector("div[class='selectWrapper']"));
                foreach(var element in dropdownElements)
                {
                    var textbox = element.FindElement(By.CssSelector("input"));
                    string placeholderValue = textbox.GetAttribute("placeholder");
                    if (placeholderValue != null && placeholderValue.Contains("luminaire type.", StringComparison.OrdinalIgnoreCase))
                    {
                        switch (LightingList[row].TypeId) {
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
                    /*if (placeholderValue != null && placeholderValue.Contains("Conditioned", StringComparison.OrdinalIgnoreCase))
                    {
                        //fill out
                    }*/
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
                }
                row++;
            }

        });
    }

    private async void Export_Click(object sender, RoutedEventArgs e)
    {
        if (LightingList.Count > 0)
        {
            Loading.Visibility = Visibility.Visible;
            StatusText.Text = "Saving";
            await db.UpdateLuminaires(LightingList);
            await ActivateSelenium();
            StatusText.Text = String.Empty;
            Loading.Visibility = Visibility.Collapsed;
        }
    }
    private async void Download_Click(object sender, RoutedEventArgs e)
    {
        LightingList.Clear();
        Loading.Visibility = Visibility.Visible;
        StatusText.Text = "Downloading";
        VersionComboBox.SelectedValue = 0;
        ProjectIds = await db.GetProjectIds(ProjectNo);
        if (ProjectIds.Count == 0)
        {
            StatusText.Text = "Project Not Found";
            Loading.Visibility = Visibility.Collapsed;
            return;
        }
        VersionComboBox.SelectedValue = ProjectIds.Keys.First();
        StatusText.Text = String.Empty;
        Loading.Visibility = Visibility.Collapsed;
        SaveProjectNo = ProjectNo;
    }
    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (LightingList.Count > 0)
        {
            Loading.Visibility = Visibility.Visible;
            StatusText.Text = "Saving";
            await db.UpdateLuminaires(LightingList);
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

        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}