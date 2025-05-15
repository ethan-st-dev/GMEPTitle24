using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPTitle24.Interior
{
    public class Lighting : INotifyPropertyChanged
    {
        private string id = Guid.NewGuid().ToString();
        private string projectId = string.Empty;
        private string tag = string.Empty;
        private string description = string.Empty;
        private bool isDecorative = false;
        private float wattage = 0;
        private int typeId = 1;
        private int wattageSourceId = 1;
        private int count = 0;
        private bool isExcluded = false;
        private int complianceMethodId = 1;
        private int occupancyTypeId = 1;
        private int conditionedTypeId = 1;
        private int conditionedQty = 0;
        private int unconditionedQty = 0;
        private int luminaireQty = 0;
        private float voltAmpRating = 0;
        private float linearFeet = 0;
        private float branchCircuitVoltage = 0;
        private float combinedBreakerAmps = 0;
        private float maxInputWattage = 0;
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
           int conditionedTypeId,
           int conditionedQty,
           int unconditionedQty,
           int luminaireQty,
           float voltAmpRating,
           float linearFeet,
           float branchCircuitVoltage,
           float combinedBreakerAmps,
           float maxInputWattage
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
            this.conditionedQty = conditionedQty;
            this.unconditionedQty = unconditionedQty;
            this.luminaireQty = luminaireQty;
            this.voltAmpRating = voltAmpRating;
            this.linearFeet = linearFeet;
            this.branchCircuitVoltage = branchCircuitVoltage;
            this.combinedBreakerAmps = combinedBreakerAmps;
            this.maxInputWattage = maxInputWattage;
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
        public int ConditionedQty
        {
            get => conditionedQty;
            set
            {
                if (conditionedQty != value)
                {
                    conditionedQty = value;
                    OnPropertyChanged(nameof(ConditionedQty));
                }
            }
        }
        public int UnconditionedQty
        {
            get => unconditionedQty;
            set
            {
                if (unconditionedQty != value)
                {
                    unconditionedQty = value;
                    OnPropertyChanged(nameof(UnconditionedQty));
                }
            }
        }
        public int LuminaireQty
        {
            get => luminaireQty;
            set
            {
                if (luminaireQty != value)
                {
                    luminaireQty = value;
                    OnPropertyChanged(nameof(LuminaireQty));
                }
            }
        }
        public float VoltAmpRating
        {
            get => voltAmpRating;
            set
            {
                if (voltAmpRating != value)
                {
                    voltAmpRating = value;
                    OnPropertyChanged(nameof(VoltAmpRating));
                }
            }
        }
        public float LinearFeet
        {
            get => linearFeet;
            set
            {
                if (linearFeet != value)
                {
                    linearFeet = value;
                    OnPropertyChanged(nameof(LinearFeet));
                }
            }
        }
        public float BranchCircuitVoltage
        {
            get => branchCircuitVoltage;
            set
            {
                if (branchCircuitVoltage != value)
                {
                    branchCircuitVoltage = value;
                    OnPropertyChanged(nameof(BranchCircuitVoltage));
                }
            }
        }
        public float CombinedBreakerAmps
        {
            get => combinedBreakerAmps;
            set
            {
                if (combinedBreakerAmps != value)
                {
                    combinedBreakerAmps = value;
                    OnPropertyChanged(nameof(CombinedBreakerAmps));
                }
            }
        }
        public float MaxInputWattage
        {
            get => maxInputWattage;
            set
            {
                if (maxInputWattage != value)
                {
                    maxInputWattage = value;
                    OnPropertyChanged(nameof(MaxInputWattage));
                }
            }
        }



        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
