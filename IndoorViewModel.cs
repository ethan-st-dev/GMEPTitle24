using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;

namespace GMEPTitle24
{
    class IndoorViewModel
    {
        public MainViewModel MainView { get; set; }
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
        public IndoorViewModel(MainViewModel MainView)
        {
            this.MainView = MainView;
            FilterBuildings();
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
        public async Task IndoorLighting()
        {
            StatusText.Text = "Navigating to Indoor Lighting";
            int result = await Task.Run(int () =>
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
            await FillOutScope();
            await FillOutLuminaires();
            await FillOutControls();
            await FillOutAllowances();
        }
        public async Task FillOutScope()
        {
            StatusText.Text = "Filling Out Scope Section";
            int result = await Task.Run(int () =>
            {
                try
                {
                    var elements = driver.FindElements(By.CssSelector("input[type='text']"));
                    foreach (var element in elements)
                    {
                        string attributeValue = element.GetAttribute("placeholder");
                        if (attributeValue != null && attributeValue.Contains("square footage of conditioned space being served by the new lighting system", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ScopeData.NewConditionedSquareFootage);
                        }
                        if (attributeValue != null && attributeValue.Contains("square footage of unconditioned space being served by the new lighting system", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ScopeData.NewUnconditionedSquareFootage);
                        }
                        if (attributeValue != null && attributeValue.Contains("square footage of conditioned space in the parking garage", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ScopeData.GarageConditionedSquareFootage);
                        }
                        if (attributeValue != null && attributeValue.Contains("square footage of unconditioned space in parking garage", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ScopeData.GarageUnconditionedSquareFootage);
                        }
                        if (attributeValue != null && attributeValue.Contains("grade stories", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ScopeData.GradeStories);
                        }
                        if (attributeValue != null && attributeValue.Contains("which tenant space or floor does this apply to", StringComparison.OrdinalIgnoreCase))
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript(@"
                            arguments[0].value = arguments[1];
                            arguments[0].dispatchEvent(new Event('input'));
                            arguments[0].dispatchEvent(new Event('change'));
                        ", element, ScopeData.ReductionComplianceSpace);
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
                            var choice = choices[ScopeData.ProjectScopeId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("in your conditioned space, which calculation method", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[ScopeData.NewConditionedMethodId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("in your unconditioned space, which calculation method", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[ScopeData.NewUnconditionedMethodId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("are there any complete floors or tenant spaces", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[ScopeData.ReductionComplianceId - 1];
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("complete building method", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[1];
                            if (ScopeData.CompleteBuildingMethod)
                            {
                                choice = choices[0];
                            }
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                        }
                        if (placeholderValue != null && placeholderValue.Contains("one-for-one luminaire alteration", StringComparison.OrdinalIgnoreCase))
                        {
                            var choices = element.FindElements(By.CssSelector("li"));
                            var choice = choices[1];
                            if (ScopeData.OneForOneAlteration)
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
                            foreach (var type in ScopeData.OccupancyTypes)
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
                            if (ScopeData.AlteredSystem)
                            {
                                var choice = choices[0];
                                var choiceLabel = choice.FindElement(By.CssSelector("label"));
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choiceLabel);
                            }
                            if (ScopeData.NewSystem)
                            {
                                var choice = choices[1];
                                var choiceLabel = choice.FindElement(By.CssSelector("label"));
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choiceLabel);
                            }
                            if (ScopeData.GarageSystem)
                            {
                                var choice = choices[2];
                                var choiceLabel = choice.FindElement(By.CssSelector("label"));
                                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choiceLabel);
                            }
                        }
                    }
                    if (ScopeData.AlteredSystem)
                    {
                        IWebElement AddSystemButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[text()='Add Altered Lighting System']")));

                        IWebElement alteredSystemContainer = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div[name='organism_Table_Row']")));
                        var Systems = alteredSystemContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));

                        foreach (var system in Systems)
                        {
                            var delete = system.FindElement(By.CssSelector("div[class='mod_supportControl']"));
                            var deleteIcon = delete.FindElement(By.CssSelector("i"));
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", deleteIcon);
                        }
                        foreach (var system in ScopeData.AlteredSystems)
                        {
                            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", AddSystemButton);

                            wait.Until(driver =>
                            {
                                var updatedSystems = alteredSystemContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));
                                return updatedSystems.Count >= ScopeData.AlteredSystems.IndexOf(system) + 1;
                            });
                        }



                        Systems = alteredSystemContainer.FindElements(By.CssSelector("div[class='mod_multiField']"));
                        //Editing Boxes
                        int row = 0;
                        foreach (var system in Systems)
                        {
                            var systemDropdownElements = system.FindElements(By.CssSelector("div[class='selectWrapper']"));
                            foreach (var element in systemDropdownElements)
                            {
                                var textbox = element.FindElement(By.CssSelector("input"));
                                string placeholderValue = textbox.GetAttribute("placeholder");
                                if (placeholderValue != null && placeholderValue.Contains("in your conditioned space, which calculation method", StringComparison.OrdinalIgnoreCase))
                                {
                                    var choices = element.FindElements(By.CssSelector("li"));
                                    var choice = choices[ScopeData.AlteredSystems[row].AlteredConditionedMethodId - 1];
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                                }
                                if (placeholderValue != null && placeholderValue.Contains("in your unconditioned space, which calculation method", StringComparison.OrdinalIgnoreCase))
                                {
                                    var choices = element.FindElements(By.CssSelector("li"));
                                    var choice = choices[ScopeData.AlteredSystems[row].AlteredUnconditionedMethodId - 1];
                                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", choice);
                                }
                            }

                            var systemElements = system.FindElements(By.CssSelector("input[type='text']"));
                            foreach (var element in systemElements)
                            {
                                string attributeValue = element.GetAttribute("placeholder");
                                if (attributeValue != null && attributeValue.Contains("square footage of conditioned space", StringComparison.OrdinalIgnoreCase))
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                                ", element, ScopeData.AlteredSystems[row].AlteredConditionedSquareFootage);
                                }
                                if (attributeValue != null && attributeValue.Contains("square footage of unconditioned space", StringComparison.OrdinalIgnoreCase))
                                {
                                    ((IJavaScriptExecutor)driver).ExecuteScript(@"
                                arguments[0].value = arguments[1];
                                arguments[0].dispatchEvent(new Event('input'));
                                arguments[0].dispatchEvent(new Event('change'));
                                ", element, ScopeData.AlteredSystems[row].AlteredUnconditionedSquareFootage);
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
                        StatusText.Text = "An error occurred while navigation to scope.";
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
        public async Task FillOutLuminaires()
        {
            StatusText.Text = "Filling Out Luminaires Section";
            int result = await Task.Run(int () =>
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

                    foreach (var lighting in LightingList)
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
                                switch (LightingList[row].ConditionedTypeId)
                                {
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
                                        ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", nonRes);
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
                                var choices = element.FindElements(By.CssSelector("li"));
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
