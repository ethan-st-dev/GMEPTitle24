using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GMEPTitle24
{
    public class Scope : INotifyPropertyChanged
    {
        public string id = Guid.NewGuid().ToString();
        public string projectId = string.Empty;
        public int projectScopeId = 1;
        public int gradeStories = 0;
        public bool completeBuildingMethod = false;
        public int newConditionedMethodId = 5;
        public int newUnconditionedMethodId = 5;
        public float newConditionedSquareFootage = 0;
        public float newUnconditionedSquareFootage = 0;
        public float garageConditionedSquareFootage = 0;
        public float garageUnconditionedSquareFootage = 0;
        public bool oneForOneAlteration = false;
        public bool alteredSystem = false;
        public bool newSystem = false;
        public bool garageSystem = false;
        public bool systemFlag = false;
        public bool completePrimaryFunctionList = false;
        private int reductionComplianceId = 3;
        private string reductionComplianceSpace = string.Empty;

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
            new CheckboxItem { Name = "High Rise Residential", Number = 9, IsSelected = false },
            new CheckboxItem { Name = "Healthcare", Number = 10, IsSelected = false },
            new CheckboxItem { Name = "Hotel/Motel", Number = 11, IsSelected = false },
            new CheckboxItem { Name = "Library Building", Number = 12, IsSelected = false },
            new CheckboxItem { Name = "Medical Clinic Building", Number = 13, IsSelected = false },
            new CheckboxItem { Name = "Multi-family/MF Mixed-user ≥4 stories (includes dormitory, senior living)", Number = 14, IsSelected = false },
            new CheckboxItem { Name = "Office Building", Number = 15, IsSelected = false },
            new CheckboxItem { Name = "Parking Garage", Number = 16, IsSelected = false },
            new CheckboxItem { Name = "Relocatable Public School", Number = 17, IsSelected = false },
            new CheckboxItem { Name = "Religious Facility Building", Number = 18, IsSelected = false },
            new CheckboxItem { Name = "Restaurant/Commercial Kitchen", Number = 19, IsSelected = false },
            new CheckboxItem { Name = "Retail Building", Number = 20, IsSelected = false },
            new CheckboxItem { Name = "School Building", Number = 21, IsSelected = false },
            new CheckboxItem { Name = "Sports Arena Building", Number = 22, IsSelected = false },
            new CheckboxItem { Name = "Support Area", Number = 23, IsSelected = false },
            new CheckboxItem { Name = "Theater Building", Number = 24, IsSelected = false },
            new CheckboxItem { Name = "Warehouse", Number = 25, IsSelected = false },
            new CheckboxItem { Name = "Other (Write In)", Number = 26, IsSelected = false }
        };
        public ObservableCollection<AlteredSystemEntry> alteredSystems = new ObservableCollection<AlteredSystemEntry>();


        public Scope(
            string id,
            string projectId,
            int projectScopeId,
            int gradeStories,
            bool oneForOneAlteration,
            bool alteredSystem,
            bool newSystem,
            bool garageSystem,
            int newConditionedMethodId,
            int newUnconditionedMethodId,
            float newConditionedSquareFootage,
            float newUnconditionedSquareFootage,
            float garageConditionedSquareFootage,
            float garageUnconditionedSquareFootage,
        List<int> occupancyTypeIds,
            ObservableCollection<AlteredSystemEntry> alteredSystems
        )
        {
            this.id = id;
            this.projectId = projectId;
            this.projectScopeId = projectScopeId;
            this.gradeStories = gradeStories;
            this.alteredSystem = alteredSystem;
            this.newSystem = newSystem;
            this.garageSystem = garageSystem;
            this.alteredSystems = alteredSystems;
            this.oneForOneAlteration = oneForOneAlteration;
            this.newConditionedMethodId = newConditionedMethodId;
            this.newUnconditionedMethodId = newUnconditionedMethodId;
            this.newConditionedSquareFootage = newConditionedSquareFootage;
            this.newUnconditionedSquareFootage = newUnconditionedSquareFootage;
            this.garageConditionedSquareFootage = garageConditionedSquareFootage;
            this.garageUnconditionedSquareFootage = garageUnconditionedSquareFootage;


            foreach (var typeId in occupancyTypeIds)
            {
                var matchingItem = OccupancyTypes.FirstOrDefault(item => item.Number == typeId);
                if (matchingItem != null)
                {
                    matchingItem.IsSelected = true;
                }
            }

            //applying listener events to alteredsystems
            foreach(var system in AlteredSystems)
            {
                system.PropertyChanged += AlteredSystem_PropertyChanged;
            }
            AlteredSystems.CollectionChanged += AlteredSystems_CollectionChanged;

            DetermineCompletePrimaryFunctionList();
            DetermineSystemFlag();
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
        public int GradeStories
        {
            get { return gradeStories; }
            set
            {
                if (gradeStories != value)
                {
                    gradeStories = value;
                    OnPropertyChanged(nameof(GradeStories));
                }
            }
        }
        public bool CompleteBuildingMethod
        {
            get { return completeBuildingMethod; }
            set
            {
                if (completeBuildingMethod != value)
                {
                    completeBuildingMethod = value;
                    OnPropertyChanged(nameof(CompleteBuildingMethod));
                    ResetCalculationMethodIds();
                    DetermineCompletePrimaryFunctionList();
                }
            }
        }
        public int NewConditionedMethodId
        {
            get { return newConditionedMethodId; }
            set
            {
                if (newConditionedMethodId != value)
                {
                    newConditionedMethodId = value;
                    OnPropertyChanged(nameof(NewConditionedMethodId));
                    DetermineCompletePrimaryFunctionList();
                }
            }
        }
        public int NewUnconditionedMethodId
        {
            get { return newUnconditionedMethodId; }
            set
            {
                if (newUnconditionedMethodId != value)
                {
                    newUnconditionedMethodId = value;
                    OnPropertyChanged(nameof(NewUnconditionedMethodId));
                    DetermineCompletePrimaryFunctionList();
                }
            }
        }
        public float NewConditionedSquareFootage
        {
            get { return newConditionedSquareFootage; }
            set
            {
                if (newConditionedSquareFootage != value)
                {
                    newConditionedSquareFootage = value;
                    OnPropertyChanged(nameof(NewConditionedSquareFootage));
                }
            }
        }
        public float NewUnconditionedSquareFootage
        {
            get { return newUnconditionedSquareFootage; }
            set
            {
                if (newUnconditionedSquareFootage != value)
                {
                    newUnconditionedSquareFootage = value;
                    OnPropertyChanged(nameof(NewUnconditionedSquareFootage));
                }
            }
        }
        public float GarageConditionedSquareFootage
        {
            get { return garageConditionedSquareFootage; }
            set
            {
                if (garageConditionedSquareFootage != value)
                {
                    garageConditionedSquareFootage = value;
                    OnPropertyChanged(nameof(GarageConditionedSquareFootage));
                }
            }
        }
        public float GarageUnconditionedSquareFootage
        {
            get { return garageUnconditionedSquareFootage; }
            set
            {
                if (garageUnconditionedSquareFootage != value)
                {
                    garageUnconditionedSquareFootage = value;
                    OnPropertyChanged(nameof(GarageUnconditionedSquareFootage));
                }
            }
        }
        public bool OneForOneAlteration
        {
            get { return oneForOneAlteration; }
            set
            {
                if (oneForOneAlteration != value)
                {
                    oneForOneAlteration = value;
                    OnPropertyChanged(nameof(OneForOneAlteration));
                    foreach(var system in AlteredSystems)
                    {
                        system.AlteredConditionedMethodId = 6;
                        system.AlteredUnconditionedMethodId = 6;
                    }
                }
            }
        }
        public bool AlteredSystem
        {
            get { return alteredSystem; }
            set
            {
                if (alteredSystem != value)
                {
                    alteredSystem = value;
                    OnPropertyChanged(nameof(AlteredSystem));
                    DetermineSystemFlag();
                    DetermineCompletePrimaryFunctionList();
                }
            }
        }
        public bool NewSystem
        {
            get { return newSystem; }
            set
            {
                if (newSystem != value)
                {
                    newSystem = value;
                    OnPropertyChanged(nameof(NewSystem));
                    DetermineSystemFlag();
                    DetermineCompletePrimaryFunctionList();
                }
            }
        }
        public bool GarageSystem
        {
            get { return garageSystem; }
            set
            {
                if (garageSystem != value)
                {
                    garageSystem = value;
                    OnPropertyChanged(nameof(GarageSystem));
                    DetermineSystemFlag();
                }
            }
        }
        public bool SystemFlag
        {
            get { return systemFlag; }
            set
            {
                if (systemFlag != value)
                {
                    systemFlag = value;
                    OnPropertyChanged(nameof(SystemFlag));
                }
            }
        }
        public bool CompletePrimaryFunctionList
        {
            get { return completePrimaryFunctionList; }
            set
            {
                if (completePrimaryFunctionList != value)
                {
                    completePrimaryFunctionList = value;
                    OnPropertyChanged(nameof(CompletePrimaryFunctionList));
                    
                }
            }
        }
        public ObservableCollection<AlteredSystemEntry> AlteredSystems
        {
            get { return alteredSystems; }
            set
            {
                if (alteredSystems != value)
                {
                    alteredSystems = value;
                    OnPropertyChanged(nameof(AlteredSystems));
                }
            }
        }
        public int ReductionComplianceId
        {
            get { return reductionComplianceId; }
            set
            {
                if (reductionComplianceId != value)
                {
                    reductionComplianceId = value;
                    OnPropertyChanged(nameof(ReductionComplianceId));
                }
            }
        }
        public string ReductionComplianceSpace
        {
            get { return reductionComplianceSpace; }
            set
            {
                if (reductionComplianceSpace != value)
                {
                    reductionComplianceSpace = value;
                    OnPropertyChanged(nameof(ReductionComplianceSpace));
                }
            }
        }
        public void DetermineSystemFlag()
        {
            if (GarageSystem || NewSystem || AlteredSystem)
            {
                SystemFlag = true;
                return;
            }
            SystemFlag = false;
        }
        public void ResetCalculationMethodIds()
        {
            NewConditionedMethodId = 5;
            NewUnconditionedMethodId = 5;
            foreach (var elem in AlteredSystems)
            {
                elem.AlteredUnconditionedMethodId = 6;
                elem.AlteredConditionedMethodId = 6;
            }
        }
        public void DetermineCompletePrimaryFunctionList()
        {
            if (NewSystem && !CompleteBuildingMethod)
            {
                if (NewConditionedMethodId == 1 || NewConditionedMethodId == 2 || NewUnconditionedMethodId == 1 || NewUnconditionedMethodId == 2)
                {
                    CompletePrimaryFunctionList = true;
                    return;
                }
                
            }
            if (AlteredSystem && !CompleteBuildingMethod)
            {
                foreach (var elem in AlteredSystems)
                {
                    if (elem.AlteredConditionedMethodId == 1 || elem.AlteredConditionedMethodId == 2 || elem.AlteredUnconditionedMethodId == 1 || elem.AlteredUnconditionedMethodId == 2)
                    {
                        CompletePrimaryFunctionList = true;
                        return;
                    }
                }
            }
            CompletePrimaryFunctionList = false;
        }
        public void AlteredSystem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is AlteredSystemEntry alteredSystem)
            {
                if (e.PropertyName == nameof(AlteredSystemEntry.AlteredConditionedMethodId) || e.PropertyName == nameof(AlteredSystemEntry.AlteredUnconditionedMethodId))
                {
                    DetermineCompletePrimaryFunctionList();
                }
            }
        }
        private void AlteredSystems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                // Attach the PropertyChanged event to new items
                foreach (AlteredSystemEntry newItem in e.NewItems)
                {
                    newItem.PropertyChanged += AlteredSystem_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                // Detach the PropertyChanged event from removed items
                foreach (AlteredSystemEntry oldItem in e.OldItems)
                {
                    oldItem.PropertyChanged -= AlteredSystem_PropertyChanged;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
    public class CheckboxItem
    {
        public string Name { get; set; }
        public int Number { get; set; }
        public bool IsSelected { get; set; }
    }
    public class AlteredSystemEntry : INotifyPropertyChanged
    {
        public string id = Guid.NewGuid().ToString();
        public string projectId = string.Empty;
        public int alteredConditionedMethodId = 5;
        public int alteredUnconditionedMethodId = 5;
        public float alteredConditionedSquareFootage = 0;
        public float alteredUnconditionedSquareFootage = 0;

        public AlteredSystemEntry(
            string id, 
            string projectId, 
            int alteredConditionedMethodId, 
            int alteredUnconditionedMethodId,
            float alteredConditionedSquareFootage,
            float alteredUnconditionedSquareFootage)
        {
            Id=id;
            ProjectId=projectId;
            AlteredConditionedMethodId=alteredConditionedMethodId;
            AlteredUnconditionedMethodId=alteredUnconditionedMethodId;
            AlteredConditionedSquareFootage=alteredConditionedSquareFootage;
            AlteredUnconditionedSquareFootage=alteredUnconditionedSquareFootage;
        }
        public AlteredSystemEntry()
        {
            //:3
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
        public int AlteredConditionedMethodId
        {
            get { return alteredConditionedMethodId; }
            set
            {
                if (alteredConditionedMethodId != value)
                {
                    alteredConditionedMethodId = value;
                    OnPropertyChanged(nameof(AlteredConditionedMethodId));
                }
            }
        }
        public int AlteredUnconditionedMethodId
        {
            get { return alteredUnconditionedMethodId; }
            set
            {
                if (alteredUnconditionedMethodId != value)
                {
                    alteredUnconditionedMethodId = value;
                    OnPropertyChanged(nameof(AlteredUnconditionedMethodId));
                }
            }
        }
        public float AlteredConditionedSquareFootage
        {
            get { return alteredConditionedSquareFootage; }
            set
            {
                if (alteredConditionedSquareFootage != value)
                {
                    alteredConditionedSquareFootage = value;
                    OnPropertyChanged(nameof(AlteredConditionedSquareFootage));
                }
            }
        }
        public float AlteredUnconditionedSquareFootage
        {
            get { return alteredUnconditionedSquareFootage; }
            set
            {
                if (alteredUnconditionedSquareFootage != value)
                {
                    alteredUnconditionedSquareFootage = value;
                    OnPropertyChanged(nameof(AlteredUnconditionedSquareFootage));
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
