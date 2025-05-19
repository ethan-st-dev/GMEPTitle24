using GMEPTitle24.Interior;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace GMEPTitle24.Exterior
{
    public class ExteriorControls : INotifyPropertyChanged
    {
        public ObservableCollection<CheckboxItem> applicationTypes = new ObservableCollection<CheckboxItem>
        {
            new CheckboxItem { Name = "General Hardscape", Number = 1, IsSelected = false },
            new CheckboxItem { Name = "ATM Machine Lighting", Number = 2, IsSelected = false },
            new CheckboxItem { Name = "Building Entrances or Exits", Number = 3, IsSelected = false },
            new CheckboxItem { Name = "Building Facades", Number = 4, IsSelected = false },
            new CheckboxItem { Name = "Drive Up Windows", Number = 5, IsSelected = false },
            new CheckboxItem { Name = "Guard Stations", Number = 6, IsSelected = false },
            new CheckboxItem { Name = "Hardscape Ornamental Lighting", Number = 7, IsSelected = false },
            new CheckboxItem { Name = "Non-Sales Canopies and Tunnels", Number = 8, IsSelected = false },
            new CheckboxItem { Name = "Outdoor Dining", Number = 9, IsSelected = false },
            new CheckboxItem { Name = "Outdoor Sales Lots", Number = 10, IsSelected = false },
            new CheckboxItem { Name = "Primary Entrances to Senior Care Facilities, Police Stations, Hospitals, Fire Stations, and Emergency Vehicle Facilities", Number = 11, IsSelected = false },
            new CheckboxItem { Name = "Sales Canopies", Number = 12, IsSelected = false },
            new CheckboxItem { Name = "Security Camera in General Hardscape > 10ft from Bldg", Number = 13, IsSelected = false },
            new CheckboxItem { Name = "Student Pick-up/Drop-off", Number = 14, IsSelected = false },
            new CheckboxItem { Name = "Vehicle Service Station Canopies", Number = 15, IsSelected = false },
            new CheckboxItem { Name = "Vehicle Service Station Hardscape", Number = 16, IsSelected = false },
            new CheckboxItem { Name = "Vehicle Service Station Uncovered Fuel Dispenser", Number = 17, IsSelected = false },
        };
    
        public ObservableCollection<CheckboxItem> ApplicationTypes
        {
            get { return applicationTypes; }
            set
            {
                if (applicationTypes != value)
                {
                    applicationTypes = value;
                    OnPropertyChanged(nameof(ApplicationTypes));
                }
            }
        }


        public ObservableCollection<CheckboxItem> checkedApplicationTypes = new ObservableCollection<CheckboxItem>();
        public ObservableCollection<CheckboxItem> CheckedApplicationTypes
        {
            get { return checkedApplicationTypes; }
            set
            {
                if (checkedApplicationTypes != value)
                {
                    checkedApplicationTypes = value;
                    OnPropertyChanged(nameof(CheckedApplicationTypes));
                }
            }
        }
        public ObservableCollection<UseOrLoseArea> useOrLoseAreas = new ObservableCollection<UseOrLoseArea>();
        public ObservableCollection<UseOrLoseArea> UseOrLoseAreas
        {
            get { return useOrLoseAreas; }
            set
            {
                if (useOrLoseAreas != value)
                {
                    useOrLoseAreas = value;
                    OnPropertyChanged(nameof(UseOrLoseAreas));
                }
            }
        }

        public ObservableCollection<HardscapeArea> hardscapeAreas = new ObservableCollection<HardscapeArea>();
        public ObservableCollection<HardscapeArea> HardscapeAreas
        {
            get { return hardscapeAreas; }
            set
            {
                if (hardscapeAreas != value)
                {
                    hardscapeAreas = value;
                    OnPropertyChanged(nameof(HardscapeAreas));
                }
            }
        }
        public ExteriorControls()
        {
            foreach (var item in ApplicationTypes)
            {
                item.PropertyChanged += CheckboxItem_PropertyChanged; ;
            }
            DetermineCheckedApplicationTypes();
            
        }

        private void CheckboxItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CheckboxItem.IsSelected))
            {
                DetermineCheckedApplicationTypes();
                DetermineEnabledTypes();
            }
        }

        public string id = Guid.NewGuid().ToString();
        public string Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }
        public string projectId = string.Empty;
        public string ProjectId
        {
            get { return projectId; }
            set
            {
                if (projectId != value)
                {
                    projectId = value;
                    OnPropertyChanged(nameof(ProjectId));
                }
            }
        }

        private int shutOffControlHandlerId = 1;
        public int ShutOffControlHandlerId
        {
            get { return shutOffControlHandlerId; }
            set
            {
                if (shutOffControlHandlerId != value)
                {
                    shutOffControlHandlerId = value;
                    OnPropertyChanged(nameof(ShutOffControlHandlerId));
                }
            }
        }
        public int timeBasedLightingControlId = 1;
        public int TimeBasedLightingControlId
        {
            get { return timeBasedLightingControlId; }
            set
            {
                if (timeBasedLightingControlId != value)
                {
                    timeBasedLightingControlId = value;
                    OnPropertyChanged(nameof(TimeBasedLightingControlId));
                }
            }
        }
        public bool luminaires20OrLess = false;
        public bool Luminaires20OrLess
        {
            get { return luminaires20OrLess; }
            set
            {
                if (luminaires20OrLess != value)
                {
                    luminaires20OrLess = value;
                    OnPropertyChanged(nameof(Luminaires20OrLess));
                }
            }
        }

        public bool hardscape = false;
        public bool Hardscape
        {
            get { return hardscape; }
            set
            {
                if (hardscape != value)
                {
                    hardscape = value;
                    OnPropertyChanged(nameof(Hardscape));
                }
            }
        }
        public bool useOrLose = false;
        public bool UseOrLose
        {
            get { return useOrLose; }
            set
            {
                if (useOrLose != value)
                {
                    useOrLose = value;
                    OnPropertyChanged(nameof(UseOrLose));
                }
            }
        }
        public bool areaFlag = false;
        public bool AreaFlag
        {
            get { return areaFlag; }
            set
            {
                if (areaFlag != value)
                {
                    areaFlag = value;
                    OnPropertyChanged(nameof(AreaFlag));
                }
            }
        }

        public void DetermineCheckedApplicationTypes()
        {
            foreach (var item in ApplicationTypes)
            {
                if (!item.IsSelected && CheckedApplicationTypes.Contains(item))
                {
                    CheckedApplicationTypes.Remove(item);
                }
                if (item.IsSelected && !CheckedApplicationTypes.Contains(item))
                {
                    CheckedApplicationTypes.Add(item);
                }
            }
        }
        public void DetermineEnabledTypes()
        {
            bool hardScapeTemp = false;
            bool useOrLoseTemp = false;

            if (ApplicationTypes[0].IsSelected)
            {
                hardScapeTemp = true;
            }

            for (int i = 1; i < ApplicationTypes.Count; i++)
            {
                if (ApplicationTypes[i].IsSelected)
                {
                    useOrLoseTemp = true;
                }
            }

            Hardscape = hardScapeTemp;
            UseOrLose = useOrLoseTemp;
            AreaFlag = useOrLoseTemp || hardScapeTemp;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
