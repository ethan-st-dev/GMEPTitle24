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
    IWebDriver driver = new ChromeDriver();
    WebDriverWait wait;

    public MainWindow()
    {
        InitializeComponent();
        ActivateSelenium();
    }
    public async Task ActivateSelenium()
    {
        Debug.WriteLine(Environment.CurrentDirectory);
        StatusText.Text = "Navigating to Site";
        await Task.Run(() =>
        {
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
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
        await GoToProject("CONICO OIL");
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

            //Reading xlsx file
            string filePath = Path.Combine(Environment.CurrentDirectory, "lti.xlsx");
            ExcelPackage.License.SetNonCommercialPersonal("<GMEP>");
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        
                int rowCount = worksheet.Dimension.Rows;
                 for (int row = 2; row <= rowCount; row++)
                 {
                     string tag = worksheet.Cells[row, 1].Text;
                     string description = worksheet.Cells[row, 2].Text;
                     string type = worksheet.Cells[row, 3].Text;
                     bool decoration = worksheet.Cells[row, 4].Text == "X";
                     string watts = worksheet.Cells[row, 5].Text;
                     string specsheet = worksheet.Cells[row, 6].Text;
                     string count = worksheet.Cells[row, 7].Text;
                     bool excluded = worksheet.Cells[row, 8].Text == "X";
                }  
            }

        });
    }
}