using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPTitle24.Exterior
{
    public class OutdoorViewModel: INotifyPropertyChanged
    {
        public ChromeOptions options;
        public IWebDriver driver;
        public WebDriverWait wait;
        public MainViewModel MainView { get; set; }

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
            ExteriorScopeData = new ExteriorScope("", "", 1, 4, 1, 0, 0, false, 0, 1, 1, [1, 2]);
            ExteriorScopeData.PropertyChanged += ExteriorScopeData_PropertyChanged;

        }
        public void ClearObjects()
        {
            ExteriorScopeData = null;
        }
        public async Task SaveObjects(string projectId)
        {
            //await MainView.db.UpdateScope(ScopeData, projectId);
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
    }
}
