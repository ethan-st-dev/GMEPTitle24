using GMEPTitle24.Interior;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPTitle24.Exterior
{
    public class ExteriorScope : INotifyPropertyChanged
    {
        public string id = Guid.NewGuid().ToString();
        public string projectId = string.Empty;
        public int projectScopeId = 1;
        public int outdoorLightingZoneId = 4;
        public int systemTypeId = 1;
        public float illuminatedHardscapeArea = 0;
        public float squareFootage = 0;
        public bool alterationIncreasedLoad = false;
        public float luminairesAltered = 0;
        public int alteredLuminairesPercentageId = 1;
        public int wattageCalculationMethodId = 1;
        public bool dwellingUnitControl = false;
        public bool multiFamily = false;
        public bool controlsEnabled = false;
        public bool isLZ0 = false;
        public ObservableCollection<CheckboxItem> OccupancyTypes { get; set; } = new ObservableCollection<CheckboxItem>
        {
            new CheckboxItem { Name = "Auditorium Building", Number = 1, IsSelected = false },
            new CheckboxItem { Name = "Classroom Building", Number = 2, IsSelected = false },
            new CheckboxItem { Name = "Commercial Industrial Building", Number = 3, IsSelected = false },
            new CheckboxItem { Name = "Convention Center Building", Number = 4, IsSelected = false },
            new CheckboxItem { Name = "Data Center Building", Number = 5, IsSelected = false },
            new CheckboxItem { Name = "Financial Institution Building", Number = 6, IsSelected = false },
            new CheckboxItem { Name = "Grocery Store Building", Number = 7, IsSelected = false },
            new CheckboxItem { Name = "Gymnasium Building", Number = 8, IsSelected = false },
            new CheckboxItem { Name = "Healthcare Facility", Number = 9, IsSelected = false },
            new CheckboxItem { Name = "Hotel/Motel", Number = 10, IsSelected = false },
            new CheckboxItem { Name = "Library Building", Number = 11, IsSelected = false },
            new CheckboxItem { Name = "Medical Clinic Building", Number = 12, IsSelected = false },
            new CheckboxItem { Name = "Multi-family/MF Mixed-user ≥4 stories (includes dormitory, senior living)", Number = 13, IsSelected = false },
            new CheckboxItem { Name = "Office Building", Number = 14, IsSelected = false },
            new CheckboxItem { Name = "Parking Garage", Number = 15, IsSelected = false },
            new CheckboxItem { Name = "Religious Facility Building", Number = 16, IsSelected = false },
            new CheckboxItem { Name = "Relocatable Public School", Number = 17, IsSelected = false },      
            new CheckboxItem { Name = "Restaurant Building", Number = 18, IsSelected = false },
            new CheckboxItem { Name = "Retail Building", Number = 19, IsSelected = false },
            new CheckboxItem { Name = "School Building", Number = 20, IsSelected = false },
            new CheckboxItem { Name = "Sports Arena Building", Number = 21, IsSelected = false },
            new CheckboxItem { Name = "Support Areas", Number = 22, IsSelected = false },
            new CheckboxItem { Name = "Theater Building", Number = 23, IsSelected = false },
            new CheckboxItem { Name = "Warehouse", Number = 24, IsSelected = false },
            new CheckboxItem { Name = "All Other Occupancies", Number = 25, IsSelected = false }
        };
        public ExteriorScope(
            string id,
            string projectId,
            int projectScopeId,
            int outdoorLightingZoneId,
            int systemTypeId,
            float illuminatedHardscapeArea,
            float squareFootage,
            bool alterationIncreasedLoad,
            float luminairesAltered,
            int alteredLuminairesPercentageId,
            int wattageCalculationMethodId,
            bool dwellingUnitControl,
            List<int> occupancyTypeIds
        )
        {
            
            this.id = id;
            this.projectId = projectId;
            this.projectScopeId = projectScopeId;
            this.systemTypeId = systemTypeId;
            this.illuminatedHardscapeArea = illuminatedHardscapeArea;
            this.squareFootage = squareFootage;
            this.alterationIncreasedLoad = alterationIncreasedLoad;
            this.luminairesAltered = luminairesAltered;
            this.alteredLuminairesPercentageId = alteredLuminairesPercentageId;
            this.wattageCalculationMethodId = wattageCalculationMethodId;
            this.outdoorLightingZoneId = outdoorLightingZoneId;
            this.dwellingUnitControl = dwellingUnitControl;
            foreach (var typeId in occupancyTypeIds)
            {
                var matchingItem = OccupancyTypes.FirstOrDefault(item => item.Number == typeId);
                if (matchingItem != null)
                {
                    matchingItem.IsSelected = true;
                }
            }
            foreach(var type in OccupancyTypes)
            {
                type.PropertyChanged += OccupancyType_PropertyChanged;
            }
            DetermineMultiFamily();
            DetermineControlsEnabled();
            DetermineIsLZ0();
        }
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
        public int ProjectScopeId
        {
            get { return projectScopeId; }
            set
            {
                if (projectScopeId != value)
                {
                    projectScopeId = value;
                    OnPropertyChanged(nameof(ProjectScopeId));
                }
            }
        }
        public bool IsLZ0
        {
            get { return isLZ0; }
            set
            {
                if (isLZ0 != value)
                {
                    isLZ0 = value;
                    OnPropertyChanged(nameof(IsLZ0));
                }
            }
        }
        public bool ControlsEnabled
        {
            get { return controlsEnabled; }
            set
            {
                if (controlsEnabled != value)
                {
                    controlsEnabled = value;
                    OnPropertyChanged(nameof(ControlsEnabled));
                }
            }
        }
        public bool MultiFamily
        {
            get { return multiFamily; }
            set
            {
                if (multiFamily != value)
                {
                    multiFamily = value;
                    OnPropertyChanged(nameof(MultiFamily));
                }
            }
        }
        public int SystemTypeId
        {
            get { return systemTypeId; }
            set
            {
                if (systemTypeId != value)
                {
                    systemTypeId = value;
                    OnPropertyChanged(nameof(SystemTypeId));
                    DetermineControlsEnabled();
                }
            }
        }
     
        public float IlluminatedHardscapeArea
        {
            get { return illuminatedHardscapeArea; }
            set
            {
                if (illuminatedHardscapeArea != value)
                {
                    illuminatedHardscapeArea = value;
                    OnPropertyChanged(nameof(IlluminatedHardscapeArea));
                }
            }
        }
        public float SquareFootage
        {
            get { return squareFootage; }
            set
            {
                if (squareFootage != value)
                {
                    squareFootage = value;
                    OnPropertyChanged(nameof(SquareFootage));
                }
            }
        }
        public bool AlterationIncreasedLoad
        {
            get { return alterationIncreasedLoad; }
            set
            {
                if (alterationIncreasedLoad != value)
                {
                    alterationIncreasedLoad = value;
                    OnPropertyChanged(nameof(AlterationIncreasedLoad));
                    DetermineControlsEnabled();
                }
            }
        }
        public float LuminairesAltered
        {
            get { return luminairesAltered; }
            set
            {
                if (luminairesAltered != value)
                {
                    luminairesAltered = value;
                    OnPropertyChanged(nameof(LuminairesAltered));
                }
            }
        }
        public int AlteredLuminairesPercentageId
        {
            get { return alteredLuminairesPercentageId; }
            set
            {
                if (alteredLuminairesPercentageId != value)
                {
                    alteredLuminairesPercentageId = value;
                    OnPropertyChanged(nameof(AlteredLuminairesPercentageId));
                }
            }
        }
        public int WattageCalculationMethodId
        {
            get { return wattageCalculationMethodId; }
            set
            {
                if (wattageCalculationMethodId != value)
                {
                    wattageCalculationMethodId = value;
                    OnPropertyChanged(nameof(WattageCalculationMethodId));
                }
            }
        }
        public int OutdoorLightingZoneId
        {
            get { return outdoorLightingZoneId; }
            set
            {
                if (outdoorLightingZoneId != value)
                {
                    outdoorLightingZoneId = value;
                    OnPropertyChanged(nameof(OutdoorLightingZoneId));
                    DetermineIsLZ0();
                }
            }
        }
        public bool DwellingUnitControl
        {
            get { return dwellingUnitControl; }
            set
            {
                if (dwellingUnitControl != value)
                {
                    dwellingUnitControl = value;
                    OnPropertyChanged(nameof(DwellingUnitControl));
                }
            }
        }


        public ObservableCollection<CheckboxItem> OccupancyTypesList
        {
            get { return OccupancyTypes; }
            set
            {
                if (OccupancyTypes != value)
                {
                    OccupancyTypes = value;
                    OnPropertyChanged(nameof(OccupancyTypesList));
                }
            }
        }
        public void DetermineMultiFamily()
        {
            if (OccupancyTypes[12].IsSelected)
            {
                MultiFamily = true;
            }
            else
            {
                MultiFamily = false;
            }
        }
        public void DetermineControlsEnabled()
        {
            bool result = true;
            if (SystemTypeId == 1 && !AlterationIncreasedLoad)
            {
                result = false;
            }
            ControlsEnabled = result;
        }
        public void DetermineIsLZ0()
        {
            IsLZ0 = (OutdoorLightingZoneId == 1);
        }
        private void OccupancyType_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CheckboxItem.IsSelected))
            {
                DetermineMultiFamily();
            }
        
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
