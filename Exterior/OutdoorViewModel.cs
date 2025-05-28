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
using System.Windows;

namespace GMEPTitle24.Exterior
{
    public class OutdoorViewModel : INotifyPropertyChanged
    {
        public ChromeOptions options;
        public IWebDriver driver;
        public WebDriverWait wait;
        public MainViewModel MainView { get; set; }

        public ObservableCollection<CheckboxItem> uplightRatings = new ObservableCollection<CheckboxItem>()
        {
            new CheckboxItem(){ Name="U0", Number=1, IsSelected=false},
            new CheckboxItem(){ Name="U1", Number=2, IsSelected=false},
            new CheckboxItem(){ Name="U2", Number=3, IsSelected=false},
            new CheckboxItem(){ Name="U3", Number=4, IsSelected=false},
            new CheckboxItem(){ Name="U4", Number=5, IsSelected=false},
        };
        public ObservableCollection<CheckboxItem> UplightRatings
        {
            get { return uplightRatings; }
            set
            {
                if (uplightRatings != value)
                {
                    uplightRatings = value;
                    OnPropertyChanged(nameof(UplightRatings));
                }
            }
        }
        public ObservableCollection<CheckboxItem> filteredUplightRatings = new ObservableCollection<CheckboxItem>();
        public ObservableCollection<CheckboxItem> FilteredUplightRatings
        {
            get { return filteredUplightRatings; }
            set
            {
                if (filteredUplightRatings != value)
                {
                    filteredUplightRatings = value;
                    OnPropertyChanged(nameof(FilteredUplightRatings));
                }
            }
        }

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

        public ExteriorControls exteriorControlsData;
        public ExteriorControls ExteriorControlsData
        {
            get { return exteriorControlsData; }
            set
            {
                if (exteriorControlsData != value)
                {
                    exteriorControlsData = value;
                    OnPropertyChanged(nameof(ExteriorControlsData));
                }
            }
        }

        public event EventHandler? ResetRows;

        public event EventHandler? ResetControlRows;
        public event EventHandler? ResetControlColumns;
        public OutdoorViewModel(MainViewModel MainView)
        {
            this.MainView = MainView;
        }
        public async Task InitializeObjects(string projectId)
        {
            ExteriorScopeData = await MainView.db.GetExteriorScope(projectId);
            ExteriorLightingList = await MainView.db.GetExteriorLighting(projectId);
            ExteriorControlsData = await MainView.db.GetExteriorControls(projectId);
            ExteriorScopeData.PropertyChanged += ExteriorScopeData_PropertyChanged;
            ExteriorControlsData.PropertyChanged += ExteriorControlsData_PropertyChanged;
            ExteriorControlsData.FilterApplicationTypes(ExteriorScopeData.OutdoorLightingZoneId);
            SetFilteredUplightRatings();
        }

        private void ExteriorControlsData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ExteriorControls.AreaFlag))
            {
                if (!ExteriorControlsData.AreaFlag) {
                    ResetControlRows?.Invoke(this, EventArgs.Empty);
                }
            }
            if (e.PropertyName == nameof(ExteriorControls.Hardscape) || e.PropertyName == nameof(ExteriorControls.UseOrLose))
            {
                if (!ExteriorControlsData.Hardscape || !ExteriorControlsData.UseOrLose)
                {
                    ResetControlColumns?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void ClearObjects()
        {
            ExteriorScopeData = null;
            ExteriorLightingList.Clear();
            ExteriorControlsData = null;
        }
        public async Task SaveObjects(string projectId)
        {
            await MainView.db.UpdateExteriorScope(ExteriorScopeData, projectId);
            await MainView.db.UpdateExteriorLuminaires(ExteriorLightingList);
            await MainView.db.UpdateExteriorControls(ExteriorControlsData, projectId);
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
            if (e.PropertyName == nameof(ExteriorScope.OutdoorLightingZoneId))
            {
                ExteriorControlsData.FilterApplicationTypes(ExteriorScopeData.OutdoorLightingZoneId);
                SetFilteredUplightRatings();
            }
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
                    IWebElement outdoorLighting = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//a[text()='NRCC-LTO-E Outdoor Lighting']")));
                    outdoorLighting.Click();
                }
                catch (WebDriverTimeoutException ex)
                {

                    MainView.StatusText = "Outdoor Lighting Section not found for project " + MainView.ProjectNo.ToString() + ".";
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
            if (result2 == false)
            {
                return false;
            }
            if (ExteriorScopeData.ControlsEnabled)
            {
                result2 = await FillOutAllowances();
                if (result2 == false)
                {
                    return false;
                }

                result2 = await FillOutControls();
            }
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
                        if (placeholderValue != null && placeholderValue.Contains("what is the % of existing luminaires being altered", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[ExteriorScopeData.AlteredLuminairesPercentageId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("which calculation method will you use to determine wattage allowance", StringComparison.OrdinalIgnoreCase))
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
                            if (placeholderValue != null && placeholderValue.Contains("backlight rating, tell us which", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[ExteriorLightingList[row].BacklightDistanceFromPropertyLineId - 1];
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                            }
                            if (placeholderValue != null && placeholderValue.Contains("what is the backlight rating", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[ExteriorLightingList[row].BacklightRatingId - 1];
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                            }
                            if (placeholderValue != null && placeholderValue.Contains("is this luminaire considered area lighting", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[1];
                                if (ExteriorLightingList[row].AreaLighting)
                                {
                                    choice = choices[0];
                                }
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
                            if (placeholderValue != null && placeholderValue.Contains("is the initial lumens for this luminaire", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[1];
                                if (ExteriorLightingList[row].MoreThan6200Lumens)
                                {
                                    choice = choices[0];
                                }
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                            }
                            if (placeholderValue != null && placeholderValue.Contains("shielding (BUG) requirements", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));
                                var choice = choices[ExteriorLightingList[row].LuminaireShieldingExceptionId];
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
    
        public async Task<bool> FillOutAllowances()
        {
            MainView.StatusText = "Filling Out Allowances Section";
            bool result = await Task.Run(bool () =>
            {
                try
                {
                    IWebElement luminaires = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Allowances']")));
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

                    var systemDropdownElements = driver.FindElements(By.CssSelector("div[class='selectWrapper']"));
                    foreach (var element in systemDropdownElements)
                    {
                        var textbox = element.FindElement(By.CssSelector("input"));
                        string placeholderValue = textbox.GetAttribute("placeholder");

                        if (placeholderValue != null && placeholderValue.Contains("application types you have on this project", StringComparison.OrdinalIgnoreCase))
                        {

                            var choices = element.FindElements(By.CssSelector("div[data-eco-field-type='checkbox']"));

                            foreach (var choice in choices)
                            {
                                var input = choice.FindElement(By.CssSelector("input"));
                                if (input.GetAttribute("checked") != null)
                                {
                                    var choiceLabel = choice.FindElement(By.CssSelector("label"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choiceLabel);
                                }
                            }
                            foreach (var type in ExteriorControlsData.ApplicationTypes)
                            {
                                if (type.IsSelected)
                                {
                                    var choice = choices[type.Number - 1];
                                    var choiceLabel = choice.FindElement(By.CssSelector("label"));
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choiceLabel);
                                }
                            }
                        }
                    }
                    if (ExteriorControlsData.Hardscape)
                    {
                        //Grabbing Container For All Lighting Entries
                        IWebElement hardscapeContainer;
                        if (ExteriorScopeData.MultiFamily)
                        {
                            hardscapeContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_NRCCLTO01E_GeneralAllowanceMF']")));
                        }
                        else
                        {
                            hardscapeContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_Section_I_TableGeneralPowerAllowance']")));
                        }

                        IWebElement AddHardscapeButton = hardscapeContainer.FindElement(By.XPath(".//div[text()='Add General Hardscape Area']"));
                        var HardscapeAreas = hardscapeContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));

                        foreach (var area in HardscapeAreas)
                        {
                            var delete = area.FindElement(By.CssSelector("div[class='mod_supportControl']"));
                            var deleteIcon = delete.FindElement(By.CssSelector("i"));
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteIcon);
                        }
                        foreach (var area in ExteriorControlsData.HardscapeAreas)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddHardscapeButton);

                            wait.Until(driver =>
                            {
                                var updatedHardscapeAreas = hardscapeContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));
                                return updatedHardscapeAreas.Count >= ExteriorControlsData.HardscapeAreas.IndexOf(area) + 1;
                            });
                        }

                        HardscapeAreas = hardscapeContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));
                        //Editing Boxes
                        int row = 0;
                        foreach (var area in HardscapeAreas)
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
                                        ", element, ExteriorControlsData.HardscapeAreas[row].Description);
                                }
                                if (attributeValue != null && attributeValue.Contains("illuminated area", StringComparison.OrdinalIgnoreCase))
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                        arguments[0].value = arguments[1];
                                        arguments[0].dispatchEvent(new Event('input'));
                                        arguments[0].dispatchEvent(new Event('change'));
                                        ", element, ExteriorControlsData.HardscapeAreas[row].Area);
                                }
                                if (attributeValue != null && attributeValue.Contains("perimeter length", StringComparison.OrdinalIgnoreCase))
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                        arguments[0].value = arguments[1];
                                        arguments[0].dispatchEvent(new Event('input'));
                                        arguments[0].dispatchEvent(new Event('change'));
                                        ", element, ExteriorControlsData.HardscapeAreas[row].PerimeterLength);
                                }
                            }
                            row++;
                        }
                    }

                    if (ExteriorControlsData.UseOrLose)
                    {
                        //Grabbing Container For All Lighting Entries
                        

                        IWebElement areaContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_BnBlWWETmlDfHnEefGrGxhgrJPBGusgV']")));
                        IWebElement areaChildContainer = areaContainer.FindElement(By.CssSelector(":scope > div.molecule_children"));
                        IWebElement AddAreaButton = areaContainer.FindElement(By.XPath(".//div[text()='Add Area']"));
                        var UseOrLoseAreas = areaChildContainer.FindElements(By.CssSelector(":scope > div.mod_multiField"));

                        foreach (var area in UseOrLoseAreas)
                        {
                            var delete = area.FindElement(By.CssSelector(":scope > div.mod_supportControl"));
                            var deleteIcon = delete.FindElement(By.CssSelector("i"));
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteIcon);
                        }
                        foreach (var area in ExteriorControlsData.UseOrLoseAreas)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddAreaButton);

                            wait.Until(driver =>
                            {
                                var updatedUseOrLoseAreas = areaChildContainer.FindElements(By.CssSelector(":scope > div.mod_multiField"));
                                return updatedUseOrLoseAreas.Count >= ExteriorControlsData.UseOrLoseAreas.IndexOf(area) + 1;
                            });
                        }

                        UseOrLoseAreas = areaChildContainer.FindElements(By.CssSelector(":scope > div.mod_multiField"));
                        //Editing Boxes
                        int row = 0;
                        foreach (var area in UseOrLoseAreas)
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
                                        ", element, ExteriorControlsData.UseOrLoseAreas[row].Description);
                                }
                                if (attributeValue != null && attributeValue.Contains("linear feet of sales frontage", StringComparison.OrdinalIgnoreCase))
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                        arguments[0].value = arguments[1];
                                        arguments[0].dispatchEvent(new Event('input'));
                                        arguments[0].dispatchEvent(new Event('change'));
                                        ", element, ExteriorControlsData.UseOrLoseAreas[row].LinearFeet);
                                }
                                if (attributeValue != null && attributeValue.Contains("specific area's square footage", StringComparison.OrdinalIgnoreCase))
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                        arguments[0].value = arguments[1];
                                        arguments[0].dispatchEvent(new Event('input'));
                                        arguments[0].dispatchEvent(new Event('change'));
                                        ", element, ExteriorControlsData.UseOrLoseAreas[row].Area);
                                }
                                if (attributeValue != null && attributeValue.Contains("number of locations", StringComparison.OrdinalIgnoreCase))
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                        arguments[0].value = arguments[1];
                                        arguments[0].dispatchEvent(new Event('input'));
                                        arguments[0].dispatchEvent(new Event('change'));
                                        ", element, ExteriorControlsData.UseOrLoseAreas[row].LocationQty);
                                }

                            }
                            var dropdownElements = area.FindElements(By.CssSelector("div[class='selectWrapper']"));
                            foreach (var element in dropdownElements)
                            {
                                var textbox = element.FindElement(By.CssSelector("input"));
                                string placeholderValue = textbox.GetAttribute("placeholder");
                                if (placeholderValue != null && placeholderValue.Contains("application type is in this area", StringComparison.OrdinalIgnoreCase))
                                {
                                    var choices = element.FindElements(By.CssSelector("li"));

                                    IWebElement choice = null;
                                    int applicationTypeId = ExteriorControlsData.UseOrLoseAreas[row].ApplicationTypeId;

                                    //Le Switcheroo (Sales Frontage & Dining)
                                    if (applicationTypeId == 9)
                                    {
                                        applicationTypeId = 10;
                                    }
                                    else if (applicationTypeId == 10)
                                    {
                                        applicationTypeId = 9;
                                    }

                                    if (applicationTypeId > 7)
                                    {
                                        choice = choices[applicationTypeId - 3];
                                    }
                                    else
                                    {
                                        if (applicationTypeId > 1)
                                        {
                                            choice = choices[applicationTypeId - 2];
                                        }

                                    }
                                    if (choice != null)
                                    {
                                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                                    }
                                }
                            }
                            row++;
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

                    MainView.StatusText = "An error occurred while navigation to luminaires.";
                    MainView.ProjectLoading = false;
                    Debug.WriteLine($"WebDriver Exception: {ex.Message}");
                    return false;
                }
                return true;
            });
            return result;
        }

        public async Task<bool> FillOutControls()
        {
            MainView.StatusText = "Filling Out Controls Section";
            bool result = await Task.Run(bool () =>
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

                    var systemDropdownElements = driver.FindElements(By.CssSelector("div[class='selectWrapper']"));
                    foreach (var element in systemDropdownElements)
                    {
                        var textbox = element.FindElement(By.CssSelector("input"));
                        string placeholderValue = textbox.GetAttribute("placeholder");
                        if (placeholderValue != null && placeholderValue.Contains("handling your shut-off controls", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            IWebElement choice = choices[ExteriorControlsData.ShutOffControlHandlerId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("will lighting be controlled with a site level time-based lighting control", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            IWebElement choice = choices[ExteriorControlsData.TimeBasedLightingControlId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("does the project only include altered lighting systems", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            IWebElement choice = choices[1];
                            if (ExteriorControlsData.Luminaires20OrLess)
                            {
                                choice = choices[0];
                            }
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                    }


                    IWebElement areaContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_OtwsEXWMAmrhfEhnDiERKDOTmBNUfKLQ']")));
                    IWebElement areaChildContainer = areaContainer.FindElement(By.CssSelector(":scope > div.molecule_children"));
                    IWebElement AddAreaButton = areaContainer.FindElement(By.XPath(".//div[text()='Add Task Area']"));
                    var Areas = areaChildContainer.FindElements(By.CssSelector(":scope > div.mod_multiField"));

                    //Removing all entries and adding new ones
                    foreach (var area in Areas)
                    {
                        var delete = area.FindElement(By.CssSelector(":scope > div.mod_supportControl"));
                        var deleteIcon = delete.FindElement(By.CssSelector("i"));
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteIcon);
                    }

                    foreach (var area in ExteriorControlsData.HardscapeAreas)
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddAreaButton);

                        // Wait for the DOM to reflect the addition of the new luminaire
                        wait.Until(driver =>
                        {
                            var updatedAreas = areaChildContainer.FindElements(By.CssSelector(":scope > div.mod_multiField"));
                            return updatedAreas.Count >= ExteriorControlsData.HardscapeAreas.IndexOf(area) + 1;
                        });
                    }
                    foreach (var area in ExteriorControlsData.UseOrLoseAreas)
                    {
                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddAreaButton);

                        // Wait for the DOM to reflect the addition of the new luminaire
                        wait.Until(driver =>
                        {
                            var updatedAreas = areaChildContainer.FindElements(By.CssSelector(":scope > div.mod_multiField"));
                            return updatedAreas.Count >= ExteriorControlsData.UseOrLoseAreas.IndexOf(area) + 1;
                        });
                    }

                    Areas = areaChildContainer.FindElements(By.CssSelector(":scope > div.mod_multiField"));
                    //Editing Boxes
                    int row = 0;
                    foreach (var area in Areas)
                    {
                        var dropdownElements = area.FindElements(By.CssSelector("div[class='selectWrapper']"));
                        foreach (var element in dropdownElements)
                        {
                            var textbox = element.FindElement(By.CssSelector("input"));
                            string placeholderValue = textbox.GetAttribute("placeholder");
                            if (placeholderValue != null && placeholderValue.Contains("allowance section where this lighting task area was defined", StringComparison.OrdinalIgnoreCase))
                            {
                                var choices = element.FindElements(By.CssSelector("li"));

                                IWebElement choice;

                                if (row < ExteriorControlsData.HardscapeAreas.Count)
                                {
                                    if (ExteriorScopeData.MultiFamily)
                                    {
                                        choice = choices[1];
                                    }
                                    else
                                    {
                                        choice  = choices[0];
                                    }
                                }
                                else
                                {
                                    choice = choices[3];
                                }

                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                            }
                        }
                        row++;
                    }

                    row = 0;
                    int hardscapeindex = 0;
                    int useOrLoseIndex = 0;
                    foreach (var area in Areas)
                    {
                        var dropdownElements = area.FindElements(By.CssSelector("div[class='selectWrapper']"));
                        foreach (var element in dropdownElements)
                        {
                            var textbox = element.FindElement(By.CssSelector("input"));
                            string placeholderValue = textbox.GetAttribute("placeholder");
                            if (placeholderValue != null && placeholderValue.Contains("choose your area", StringComparison.OrdinalIgnoreCase))
                            {
                                IWebElement Parent = element.FindElement(By.XPath(".."));
                                if (!Parent.GetAttribute("class").Contains("hideForDependency"))
                                {
                                    var choices = element.FindElements(By.CssSelector("li"));

                                    IWebElement choice;
                                    if (row < ExteriorControlsData.HardscapeAreas.Count)
                                    {
                                        choice = choices[hardscapeindex];
                                        hardscapeindex++;
                                    }
                                    else
                                    {
                                        choice = choices[useOrLoseIndex];
                                        useOrLoseIndex++;
                                    }

                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
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
        public void SetFilteredUplightRatings()
        {
            foreach (var entry in UplightRatings)
            {
                if (ExteriorScopeData.OutdoorLightingZoneId - 1 >= entry.Number)
                {
                    if (!FilteredUplightRatings.Contains(entry))
                    {
                        FilteredUplightRatings.Add(entry);
                    }
                }
                else
                {
                    if (FilteredUplightRatings.Contains(entry))
                    {
                        FilteredUplightRatings.Remove(entry);
                    }
                }
            }
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
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return b ? Visibility.Collapsed: Visibility.Visible;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
                return v == Visibility.Collapsed;
            return false;
        }
    }
    public class BacklightRatingIdToNameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: BacklightRatingId (int)
            // values[1]: FilteredBacklightRatings (ObservableCollection<CheckboxItem>)
            int id = 0;
            if (values.Length > 0 && values[0] is int intId)
                id = intId;

            var ratings = values.Length > 1 ? values[1] as ObservableCollection<CheckboxItem> : null;
            if (ratings != null)
            {
                var item = ratings.FirstOrDefault(r => r.Number == id);
                return item?.Name ?? id.ToString();
            }
            return id.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
