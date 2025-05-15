using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace GMEPTitle24.Outdoor
{

    public class OutdoorViewModel : INotifyPropertyChanged
    {
        public ChromeOptions options;
        public IWebDriver driver;
        public WebDriverWait wait;
        public MainViewModel MainView { get; set; }
        public OutdoorViewModel(MainViewModel MainView)
        {
            this.MainView = MainView;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
