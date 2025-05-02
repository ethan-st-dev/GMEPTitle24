using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPTitle24
{
    public class Scope : INotifyPropertyChanged
    {
        private string id = Guid.NewGuid().ToString();
        private string projectId = string.Empty;
        private HashSet<int> occupancyTypeIds = new HashSet<int>();
        private int scopeTypeId = 1;
        private HashSet<int> lightingSystemIds = new HashSet<int>();
        private int gradeStories = 1;
        bool completedBuildingMethod = true;
        private int conditionedMethodId = 1;
        private int unconditionedMethodId = 1;
        private float squareFootage = 0;

        public event PropertyChangedEventHandler PropertyChanged;
        public Lighting(
           string id,
           string projectId,
           string tag,
           string description,
           bool isDecorative,
           float wattage,
           int wattageSourceId,
           int typeId,
           int count,
           bool isExcluded,
           int complianceMethodId,
           int occupancyTypeId,
           int conditionedTypeId
        )
        {
            this.id = id;
            this.projectId = projectId;
            this.tag = tag;
            this.description = description;
            this.isDecorative = isDecorative;
            this.wattage = wattage;
            this.wattageSourceId = wattageSourceId;
            this.typeId = typeId;
            this.count = count;
            this.isExcluded = isExcluded;
            this.complianceMethodId = complianceMethodId;
            this.occupancyTypeId = occupancyTypeId;
            this.conditionedTypeId = conditionedTypeId;
        }
        public string Id
        {
            get => id;
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
            get => projectId;
            set
            {
                if (projectId != value)
                {
                    projectId = value;
                    OnPropertyChanged(nameof(ProjectId));
                }
            }
        }
        public string Tag
        {
            get => tag;
            set
            {
                if (tag != value)
                {
                    tag = value;
                    OnPropertyChanged(nameof(Tag));
                }
            }
        }
        public string Description
        {
            get => description;
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public bool IsDecorative
        {
            get => isDecorative;
            set
            {
                if (isDecorative != value)
                {
                    isDecorative = value;
                    OnPropertyChanged(nameof(IsDecorative));
                }
            }
        }
        public float Wattage
        {
            get => wattage;
            set
            {
                if (wattage != value)
                {
                    wattage = value;
                    OnPropertyChanged(nameof(Wattage));
                }
            }
        }
        public int WattageSourceId
        {
            get => wattageSourceId;
            set
            {
                if (wattageSourceId != value)
                {
                    wattageSourceId = value;
                    OnPropertyChanged(nameof(WattageSourceId));
                }
            }
        }
        public int TypeId
        {
            get => typeId;
            set
            {
                if (typeId != value)
                {
                    typeId = value;
                    OnPropertyChanged(nameof(TypeId));
                }
            }
        }
        public int Count
        {
            get => count;
            set
            {
                if (count != value)
                {
                    count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }
        public bool IsExcluded
        {
            get => isExcluded;
            set
            {
                if (isExcluded != value)
                {
                    isExcluded = value;
                    OnPropertyChanged(nameof(IsExcluded));
                }
            }
        }
        public int ComplianceMethodId
        {
            get => complianceMethodId;
            set
            {
                if (complianceMethodId != value)
                {
                    complianceMethodId = value;
                    OnPropertyChanged(nameof(ComplianceMethodId));
                }
            }
        }
        public int OccupancyTypeId
        {
            get => occupancyTypeId;
            set
            {
                if (occupancyTypeId != value)
                {
                    occupancyTypeId = value;
                    OnPropertyChanged(nameof(OccupancyTypeId));
                }
            }
        }
        public int ConditionedTypeId
        {
            get => conditionedTypeId;
            set
            {
                if (conditionedTypeId != value)
                {
                    conditionedTypeId = value;
                    OnPropertyChanged(nameof(ConditionedTypeId));
                }
            }
        }



        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
