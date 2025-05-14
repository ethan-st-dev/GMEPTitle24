using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Interactions;
using SeleniumExtras.WaitHelpers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using System.Windows;

namespace GMEPTitle24
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ChromeOptions options;
        public IWebDriver driver;
        public WebDriverWait wait;
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

        public string statusText = string.Empty;

        public string StatusText
        {
            get { return statusText; }
            set
            {
                statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }
        public bool projectLoading = false;
        public bool ProjectLoading
        {
            get { return projectLoading; }
            set
            {
                projectLoading = value;
                OnPropertyChanged(nameof(ProjectLoading));
            }
        }
        public bool projectLoaded = false;
        public bool ProjectLoaded
        {
            get { return projectLoaded; }
            set
            {
                projectLoaded = value;
                OnPropertyChanged(nameof(ProjectLoaded));
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

            StatusText = "Navigating to Site";

            int result = await Task.Run(int () =>
            {
                try
                {
                    wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    driver.Navigate().GoToUrl("https://energycodeace.com/");
                }
                catch (WebDriverTimeoutException ex)
                {

                    StatusText = "Navigation timed out. Please try again.";
                    ProjectLoading = false;
                    Debug.WriteLine($"Timeout Exception: {ex.Message}");
                    return 0;
                }
                catch (WebDriverException ex)
                {
                    StatusText = "An error occurred while navigating to the site.";
                    ProjectLoading = false;
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
            
        }

        public async Task Login()
        {
            StatusText = "Logging In";
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

                    StatusText = "Navigation timed out. Please try again.";
                    ProjectLoading = false;
                    Debug.WriteLine($"Timeout Exception: {ex.Message}");
                    return 0;
                }
                catch (WebDriverException ex)
                {
                    StatusText = "An error occurred while logging in.";
                    ProjectLoading = false;
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
            StatusText = "Navigating To Project";

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
                    StatusText = "Project not loaded or found. Please create a project starting with the project number " + projectNo.ToString() + ".";
                    ProjectLoading = false;
                    Debug.WriteLine($"Timeout Exception: {ex.Message}");
                    return 0;
                }
                catch (WebDriverException ex)
                {
                    StatusText = "An error occurred while navigating to project.";
                    ProjectLoading = false;
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
        public async Task LaunchWindow()
        {
            var url = driver.Url;
            driver.Quit();

            StatusText= "Launching Window.";
            ProjectLoading = false;

            await Task.Run(() =>
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true // Ensures the default browser is used
                });
            });
            StatusText = "";
            ProjectLoading = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
