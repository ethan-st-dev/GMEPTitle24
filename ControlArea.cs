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
        private int primaryFunctionId = 18;
        private int areaControlTypeId = 3;
        private int multilevelControlTypeId = 1;
        private int shutoffControlTypeId = 1;
        private int primaryDaylightControlTypeId = 1;
        private int secondaryDaylightControlTypeId = 1;
        private bool interlockedSystems = false;
        private float squareFootage = 0;

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
            float squareFootage
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
        }
        public ControlArea()
        {
            // Default constructor :3
        }
    }
}
