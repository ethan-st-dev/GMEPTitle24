using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeleniumExtras.WaitHelpers;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Collections.ObjectModel;
using GMEPTitle24.Interior;

namespace GMEPTitle24.Exterior
{
    public class OutdoorViewModel: INotifyPropertyChanged
    {
        public ChromeOptions options;
        public IWebDriver driver;
        public WebDriverWait wait;
        public MainViewModel MainView { get; set; }

        public ObservableCollection<ExteriorLighting> exteriorLightingList = new ObservableCollection<ExteriorLighting>();
        public ObservableCollection<ExteriorLighting> ExteriorLightingList
        {
            get { return exteriorLightingList; }
            set
            {
                if (exteriorLightingList != value)
                {
                    exteriorLightingList = value;
                    OnPropertyChanged(nameof(ExteriorLightingList));
                }
            }
        }


        public ExteriorScope exteriorScopeData;
        public ExteriorScope ExteriorScopeData
        {
            get { return exteriorScopeData; }
            set
            {
                if (exteriorScopeData != value)
                {
                    exteriorScopeData = value;
                    OnPropertyChanged(nameof(ExteriorScopeData));
                }
            }
        }

        public event EventHandler? ResetRows;
        public OutdoorViewModel(MainViewModel MainView)
        {
            this.MainView = MainView;
        }
        public async Task InitializeObjects(string projectId)
        {
            ExteriorScopeData = await MainView.db.GetExteriorScope(projectId);
            ExteriorLightingList = await MainView.db.GetExteriorLighting(projectId);
            ExteriorScopeData.PropertyChanged += ExteriorScopeData_PropertyChanged;

        }
        public void ClearObjects()
        {
            ExteriorScopeData = null;
        }
        public async Task SaveObjects(string projectId)
        {
            await MainView.db.UpdateExteriorScope(ExteriorScopeData, projectId);
            await MainView.db.UpdateExteriorLuminaires(ExteriorLightingList);
        }
        private void ExteriorScopeData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExteriorScope.SystemTypeId))
            {
                if (ExteriorScopeData.SystemTypeId == 2)
                {
                    ResetRows?.Invoke(this, EventArgs.Empty);
                }
            }
            /*if (e.PropertyName == nameof(Scope.CompletePrimaryFunctionList))
            {
                FilterBuildings();
                ResetPrimaryFunctionIds();
            }*/
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Selenium Search Functions
        public async Task<bool> OutdoorLighting()
        {
            options = MainView.options;
            driver = MainView.driver;
            wait = MainView.wait;
            MainView.StatusText = "Navigating to Outdoor Lighting";
            bool result = await Task.Run(bool () =>
            {
                try
                {
                    IWebElement indoorLighting = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[text()='NRCC-LTO-E Outdoor Lighting']")));
                    indoorLighting.Click();
                }
                catch (WebDriverTimeoutException ex)
                {

                    MainView.StatusText = "Outdoor Lighting Section not found. Please try again.";
                    MainView.ProjectLoading = false;

                    Debug.WriteLine($"Timeout Exception: {ex.Message}");
                    return false;
                }
                catch (WebDriverException ex)
                {
                    MainView.StatusText = "An error occurred while logging in.";
                    MainView.ProjectLoading = false;
                    Debug.WriteLine($"WebDriver Exception: {ex.Message}");
                    return false;
                }
                return true;
            });
            if (result == false)
            {
                return false;
            }

            bool result2 = await FillOutScope();
            if (result2 == false)
            {
                return false;
            }

            result2 = await FillOutLuminaires();
            /*if (result2 == false)
            {
                return false;
            }*/
           /* result2 = await FillOutControls();
            if (result2 == false)
            {
                return false;
            }
            result2 = await FillOutAllowances();*/
            return result2;
        }

        public async Task<bool> FillOutScope()
        {
            MainView.StatusText = "Filling Out Scope Section";
            bool result = await Task.Run(bool () =>
            {
                try
                {
                    var elements = driver.FindElements(By.CssSelector("input[type='text']"));
                    foreach (var element in elements)
                    {
                        string attributeValue = element.GetAttribute("placeholder");
                        if (attributeValue != null && attributeValue.Contains("square footage of the building", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ExteriorScopeData.SquareFootage);
                        }
                        if (attributeValue != null && attributeValue.Contains("total illuminated hardscape area", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ExteriorScopeData.IlluminatedHardscapeArea);
                        }
                        if (attributeValue != null && attributeValue.Contains("sum total of luminaires being added or altered", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ExteriorScopeData.LuminairesAltered);
                        }
                    }
                    var dropdownElements = driver.FindElements(By.CssSelector("div[class='selectWrapper']"));
                    foreach (var element in dropdownElements)
                    {
                        var textbox = element.FindElement(By.CssSelector("input"));
                        string placeholderValue = textbox.GetAttribute("placeholder");
                        if (placeholderValue != null && placeholderValue.Contains("project's scope", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[ExteriorScopeData.ProjectScopeId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("outdoor lighting zone", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[ExteriorScopeData.OutdoorLightingZoneId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("what is the % of the existing luminaires being altered", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[ExteriorScopeData.AlteredLuminairesPercentageId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("what calculation method will you use to determine wattage allowance", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[ExteriorScopeData.WattageCalculationMethodId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }

                        if (placeholderValue != null && placeholderValue.Contains("alteration increasing the connected lighting load (watts)", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[1];
                            if (ExteriorScopeData.AlterationIncreasedLoad)
                            {
                                choice = choices[0];
                            }
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("outdoor lighting that is controlled from within a dwelling unit", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[1];
                            if (ExteriorScopeData.DwellingUnitControl)
                            {
                                choice = choices[0];
                            }
                          ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("occupancy types", StringComparison.OrdinalIgnoreCase))
                        {

                            var choices = element.FindElements(By.CssSelector("div[data-eco-field-type='checkbox'"));

                            foreach (var choice in choices)
                            {
                                var input = choice.FindElement(By.CssSelector("input"));
                                if (input.GetAttribute("checked") != null)
                                {
                                    var choiceLabel = choice.FindElement(By.CssSelector("label"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choiceLabel);
                                }
                            }
                            foreach (var type in ExteriorScopeData.OccupancyTypes)
                            {
                                if (type.IsSelected)
                                {
                                    var choice = choices[type.Number - 1];
                                    var choiceLabel = choice.FindElement(By.CssSelector("label"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choiceLabel);
                                }
                            }
                        }
                        if (placeholderValue != null && placeholderValue.Contains("lighting system will be installed", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[ExteriorScopeData.SystemTypeId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
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

                    MainView.StatusText = "Navigation timed out. Please try again.";
                    MainView.ProjectLoading = false;
                    Debug.WriteLine($"Timeout Exception: {ex.Message}");
                    return false;
                }
                catch (WebDriverException ex)
                {
                    MainView.StatusText = "An error occurred while navigation to scope.";
                    MainView.ProjectLoading = false;
                    Debug.WriteLine($"WebDriver Exception: {ex.Message}");
                    return false;
                }
                return true;
            });
            return result;
        }

        public async Task<bool> FillOutLuminaires()
        {
            MainView.StatusText = "Filling Out Luminaires Section";
            bool result = await Task.Run(bool () =>
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

                    IWebElement lightingContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_wufmffESvduAkDmTkpmftBXKkRZerUUP']")));
                    var Lightings = lightingContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));

                    //Removing all entries and adding new ones
                    foreach (var lighting in Lightings)
                    {
                        var delete = lighting.FindElement(By.CssSelector("div[class='mod_supportControl']"));
                        var deleteIcon = delete.FindElement(By.CssSelector("i"));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteIcon);
                    }

                    foreach (var lighting in ExteriorLightingList)
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddLuminaireButton);

                        // Wait for the DOM to reflect the addition of the new luminaire
                        wait.Until(driver =>
                        {
                            var updatedLightings = lightingContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));
                            return updatedLightings.Count >= ExteriorLightingList.IndexOf(lighting) + 1;
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
                                ", element, ExteriorLightingList[row].Tag);
                            }
                            if (attributeValue != null && attributeValue.Contains("luminaire description", StringComparison.OrdinalIgnoreCase))
                            {
                                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                                ", element, ExteriorLightingList[row].Description);
                            }
                            if (attributeValue != null && attributeValue.Contains("watts per linear foot", StringComparison.OrdinalIgnoreCase))
                            {
                                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                                ", element, ExteriorLightingList[row].Wattage);
                            }
                            if (attributeValue != null && attributeValue.Contains("watts per luminaire", StringComparison.OrdinalIgnoreCase))
                            {
                                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                                ", element, ExteriorLightingList[row].Wattage);
                            }
                            if (attributeValue != null && attributeValue.Contains("how many luminaires are being replaced", StringComparison.OrdinalIgnoreCase))
                            {
                                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                                ", element, ExteriorLightingList[row].LuminaireQty);
                            }
                            if (attributeValue != null && attributeValue.Contains("number of luminiares", StringComparison.OrdinalIgnoreCase))
                            {
                                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                                ", element, ExteriorLightingList[row].LuminaireQty);
                            }
                            if (attributeValue != null && attributeValue.Contains("total linear feet", StringComparison.OrdinalIgnoreCase))
                            {
                                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                                ", element, ExteriorLightingList[row].TotalLinearFeet);
                            }
                            if (attributeValue != null && attributeValue.Contains("other method of determining compliance", StringComparison.OrdinalIgnoreCase))
                            {
                                ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                                ", element, ExteriorLightingList[row].OtherComplianceMethodDescription);
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
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[ExteriorLightingList[row].TypeId - 1];
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                            }
                            if (placeholderValue != null && placeholderValue.Contains("wattage determined", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[ExteriorLightingList[row].WattageDeterminedOptionId - 1];
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                            }
                            if (placeholderValue != null && placeholderValue.Contains("which option describes this luminaire", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[ExteriorLightingList[row].DescriptionOptionId - 1];
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                            }
                            if (placeholderValue != null && placeholderValue.Contains("mounting height", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[ExteriorLightingList[row].MountingTypeId - 1];
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                            }
                            if (placeholderValue != null && placeholderValue.Contains("luminaire type excluded", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[0];
                                if (ExteriorLightingList[row].Excluded)
                                {
                                    choice = choices[1];
                                }
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                            }
                            if (placeholderValue != null && placeholderValue.Contains("is this linear lighting", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[1];
                                if (ExteriorLightingList[row].DescriptionOptionId == 1)
                                {
                                    choice = choices[0];
                                }
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
                    MainView.StatusText = "Navigation timed out. Please try again.";
                    MainView.ProjectLoading = false;
                    Debug.WriteLine($"Timeout Exception: {ex.Message}");
                    return false;
                }
                catch (WebDriverException ex)
                {

                    MainView.StatusText = "An error occurred while navigation to luminaires.";
                    MainView.ProjectLoading = false;
                    Debug.WriteLine($"WebDriver Exception: {ex.Message}");
                    return false;
                }
                return true;
            });
            return result;
        }
    }
    public class IsFiveOrHigherConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is float val)
            {
                if (val >= 5)
                {
                    return true;
                }
            }
            return false;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture
        )
        {
            throw new NotImplementedException();
        }
    }
}
