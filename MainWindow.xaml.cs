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


namespace GMEPTitle24;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    ChromeOptions options;
    IWebDriver driver;
    WebDriverWait wait;

    public MainWindow()
    {
        InitializeComponent();
    }
    public async Task ActivateSelenium()
    {
        options = new ChromeOptions();
        //options.AddArgument("headless");
        driver = new ChromeDriver(options);
        Loading.Visibility = Visibility.Visible;
        StatusText.Text = "Navigating to Site";
        await Task.Run(() =>
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            driver.Navigate().GoToUrl("https://energycodeace.com/");
        });
        await Login();

        //Quitting Program
        //driver.Quit();
        Loading.Visibility = Visibility.Collapsed;
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
        await GoToProject("24-303");
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

            //Reading xlsx file
            string filePath = Path.Combine(Environment.CurrentDirectory, "lti.xlsx");
            ExcelPackage.License.SetNonCommercialPersonal("<GMEP>");

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                int lightingCount = Lightings.Count;

                //Adjusting Box Count
                if (lightingCount < rowCount - 1)
                {    //Adding Boxes
                    while (lightingCount < rowCount - 1)
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddLuminaireButton);
                        lightingCount++;
                    }
                }
                else if (lightingCount > rowCount - 1)
                {
                    //Removing Boxes
                    foreach (var lighting in Lightings)
                    {
                        if (lightingCount > rowCount - 1)
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
                int row = 2;
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
                            ", element, worksheet.Cells[row, 1].Text);
                        }
                        if (attributeValue != null && attributeValue.Contains("luminaire description", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                            ", element, worksheet.Cells[row, 2].Text);
                        }
                        if (attributeValue != null && attributeValue.Contains("Watts per Luminaire", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                            ", element, worksheet.Cells[row, 5].Text);
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
                            switch (worksheet.Cells[row, 3].Text.ToLower()) {
                                case "individual luminaire":
                                    var individualLuminaire = element.FindElement(By.CssSelector("li[data-value='TypicalLuminaire'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", individualLuminaire);
                                    break;
                                case "track lighting":
                                    var trackLighting = element.FindElement(By.CssSelector("li[data-value='TrackLighting'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", trackLighting);
                                    break;
                                case "small aperature and color changing":
                                    var smallAperatureColor = element.FindElement(By.CssSelector("li[data-value='SmallAperatureColor'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", smallAperatureColor);
                                    break;
                            }
                        }
                        if (placeholderValue != null && placeholderValue.Contains("general lighting", StringComparison.OrdinalIgnoreCase))
                        {
                            switch (worksheet.Cells[row, 4].Text.ToLower())
                            {
                                case "x":
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
                            switch (worksheet.Cells[row, 6].Text.ToLower())
                            {
                                case "manufacturer spec sheet":
                                    var ManufacturerSpec = element.FindElement(By.CssSelector("li[data-value='ManufacturerSpec'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", ManufacturerSpec);
                                    break;
                                case "default":
                                    var Default = element.FindElement(By.CssSelector("li[data-value='CEC_Default'"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", Default);
                                    break;
                                case "other":
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
                            switch (worksheet.Cells[row, 8].Text.ToLower())
                            {
                                case "x":
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
            }

        });
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        ActivateSelenium();
    }
}