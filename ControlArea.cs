using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPTitle24
{
    public class ControlArea : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string id = Guid.NewGuid().ToString();
        private string projectId = string.Empty;
        private string description = string.Empty;
        private int primaryFunctionId = 93;
        private int areaControlTypeId = 3;
        private int multilevelControlTypeId = 1;
        private int shutoffControlTypeId = 1;
        private int primaryDaylightControlTypeId = 1;
        private int secondaryDaylightControlTypeId = 1;
        private bool interlockedSystems = false;
        private float squareFootage = 0;
        private bool conditioned = true;
        private int powerAdjustmentId = 2;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        public string Description
        {
            get { return description; }
            set
            {
                if (description != value)
                {
                    description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
        public int PrimaryFunctionId
        {
            get { return primaryFunctionId; }
            set
            {
                if (primaryFunctionId != value)
                {
                    primaryFunctionId = value;
                    OnPropertyChanged(nameof(PrimaryFunctionId));
                    SetDefaults();
                }
            }
        }
        public int AreaControlTypeId
        {
            get { return areaControlTypeId; }
            set
            {
                if (areaControlTypeId != value)
                {
                    areaControlTypeId = value;
                    OnPropertyChanged(nameof(AreaControlTypeId));
                }
            }
        }
        public int MultilevelControlTypeId
        {
            get { return multilevelControlTypeId; }
            set
            {
                if (multilevelControlTypeId != value)
                {
                    multilevelControlTypeId = value;
                    OnPropertyChanged(nameof(MultilevelControlTypeId));
                }
            }
        }
        public int ShutoffControlTypeId
        {
            get { return shutoffControlTypeId; }
            set
            {
                if (shutoffControlTypeId != value)
                {
                    shutoffControlTypeId = value;
                    OnPropertyChanged(nameof(ShutoffControlTypeId));
                }
            }
        }
        public int PrimaryDaylightControlTypeId
        {
            get { return primaryDaylightControlTypeId; }
            set
            {
                if (primaryDaylightControlTypeId != value)
                {
                    primaryDaylightControlTypeId = value;
                    OnPropertyChanged(nameof(PrimaryDaylightControlTypeId));
                }
            }
        }
        public int SecondaryDaylightControlTypeId
        {
            get { return secondaryDaylightControlTypeId; }
            set
            {
                if (secondaryDaylightControlTypeId != value)
                {
                    secondaryDaylightControlTypeId = value;
                    OnPropertyChanged(nameof(SecondaryDaylightControlTypeId));
                }
            }
        }
        public bool InterlockedSystems
        {
            get { return interlockedSystems; }
            set
            {
                if (interlockedSystems != value)
                {
                    interlockedSystems = value;
                    OnPropertyChanged(nameof(InterlockedSystems));
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
        public bool Conditioned
        {
            get { return conditioned; }
            set
            {
                if (conditioned != value)
                {
                    conditioned = value;
                    OnPropertyChanged(nameof(Conditioned));
                }
            }
        }
        public int PowerAdjustmentId
        {
            get { return powerAdjustmentId; }
            set
            {
                if (powerAdjustmentId != value)
                {
                    powerAdjustmentId = value;
                    OnPropertyChanged(nameof(PowerAdjustmentId));
                }
            }
        }
        public ControlArea(
            string id,
            string projectId,
            string description,
            int primaryFunctionId,
            int areaControlTypeId,
            int multilevelControlTypeId,
            int shutoffControlTypeId,
            int primaryDaylightControlTypeId,
            int secondaryDaylightControlTypeId,
            bool interlockedSystems,
            float squareFootage,
            bool conditioned,
            int powerAdjustmentId
        )
        {
            this.id = id;
            this.projectId = projectId;
            this.description = description;
            this.primaryFunctionId = primaryFunctionId;
            this.areaControlTypeId = areaControlTypeId;
            this.multilevelControlTypeId = multilevelControlTypeId;
            this.shutoffControlTypeId = shutoffControlTypeId;
            this.primaryDaylightControlTypeId = primaryDaylightControlTypeId;
            this.secondaryDaylightControlTypeId = secondaryDaylightControlTypeId;
            this.interlockedSystems = interlockedSystems;
            this.squareFootage = squareFootage;
            this.conditioned = conditioned;
            this.powerAdjustmentId = powerAdjustmentId;
        }
        public ControlArea()
        {
            // Default constructor :3
        }
        public void SetDefaults()
        {
            switch(PrimaryFunctionId){
                case 74:
                case 75:
                    MultilevelControlTypeId = 9;
                    ShutoffControlTypeId = 5;
                    PrimaryDaylightControlTypeId = 7;
                    SecondaryDaylightControlTypeId= 7;
                    break;
            }
        }
    }
}
