using System.ComponentModel;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Printing;

namespace GMEPTitle24.Exterior
{
    public class ExteriorLighting : INotifyPropertyChanged
    {
        private string id = Guid.NewGuid().ToString();
        private string projectId = string.Empty;
        private string tag = string.Empty;
        private string description = string.Empty;
        private int typeId = 1;
        private int wattageDeterminedOptionId = 1;
        private int descriptionOptionId = 1;
        private float wattage = 0;
        private float totalLinearFeet = 0;
        private float luminaireQty = 0;
        private int mountingTypeId = 1;
        private bool excluded = false;
        private bool moreThan6200Lumens = false;
        private int luminaireShieldingExceptionId = 1;
        private string otherComplianceMethodDescription = string.Empty;

        public event PropertyChangedEventHandler PropertyChanged;
        public ExteriorLighting(
            string id,
            string projectId,
            string tag,
            string description,
            int typeId,
            int wattageDeterminedOptionId,
            int descriptionOptionId,
            float wattage,
            float totalLinearFeet,
            float luminaireQty,
            int mountingTypeId,
            bool excluded,
            bool moreThan6200Lumens,
            int luminaireShieldingExceptionId,
            string otherComplianceMethodDescription
        )
        {
            this.id = id;
            this.projectId = projectId;
            this.tag = tag;
            this.description = description;
            this.typeId = typeId;
            this.wattageDeterminedOptionId = wattageDeterminedOptionId;
            this.descriptionOptionId = descriptionOptionId;
            this.wattage = wattage;
            this.totalLinearFeet = totalLinearFeet;
            this.luminaireQty= luminaireQty;
            this.mountingTypeId = mountingTypeId;
            this.excluded = excluded;
            this.moreThan6200Lumens = moreThan6200Lumens;
            this.luminaireShieldingExceptionId = luminaireShieldingExceptionId;
            this.otherComplianceMethodDescription = otherComplianceMethodDescription;
        }

        //EMpty constructor for testing
        public ExteriorLighting()
        {
            //:3
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
        public int WattageDeterminedOptionId
        {
            get => wattageDeterminedOptionId;
            set
            {
                if (wattageDeterminedOptionId != value)
                {
                    wattageDeterminedOptionId = value;
                    OnPropertyChanged(nameof(WattageDeterminedOptionId));
                }
            }
        }
        public int DescriptionOptionId
        {
            get => descriptionOptionId;
            set
            {
                if (descriptionOptionId != value)
                {
                    descriptionOptionId = value;
                    OnPropertyChanged(nameof(DescriptionOptionId));
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

        public float TotalLinearFeet
        {
            get => totalLinearFeet;
            set
            {
                if (totalLinearFeet != value)
                {
                    totalLinearFeet = value;
                    OnPropertyChanged(nameof(TotalLinearFeet));
                }
            }
        }
        public float LuminaireQty
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
        public int MountingTypeId
        {
            get => mountingTypeId;
            set
            {
                if (mountingTypeId != value)
                {
                    mountingTypeId = value;
                    OnPropertyChanged(nameof(MountingTypeId));
                }
            }
        }
        public bool Excluded
        {
            get => excluded;
            set
            {
                if (excluded != value)
                {
                    excluded = value;
                    OnPropertyChanged(nameof(Excluded));
                }
            }
        }
        public bool MoreThan6200Lumens
        {
            get => moreThan6200Lumens;
            set
            {
                if (moreThan6200Lumens != value)
                {
                    moreThan6200Lumens = value;
                    OnPropertyChanged(nameof(MoreThan6200Lumens));
                }
            }
        }
        public int LuminaireShieldingExceptionId
        {
            get => luminaireShieldingExceptionId;
            set
            {
                if (luminaireShieldingExceptionId != value)
                {
                    luminaireShieldingExceptionId = value;
                    OnPropertyChanged(nameof(LuminaireShieldingExceptionId));
                }
            }
        }
        public string OtherComplianceMethodDescription
        {
            get => otherComplianceMethodDescription;
            set
            {
                if (otherComplianceMethodDescription != value)
                {
                    otherComplianceMethodDescription = value;
                    OnPropertyChanged(nameof(OtherComplianceMethodDescription));
                }
            }
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
