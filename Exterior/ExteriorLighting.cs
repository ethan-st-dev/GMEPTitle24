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
        private float wattagePerLuminaire = 0;
        private float wattagePerLinearFoot = 0;
        private float totalLinearFeet = 0;
        private float numLuminaires = 0;
        private int mountingTypeId = 1;
        private bool excluded = false;
        private bool moreThan6200Lumens = false;
        private int luminaireShieldingExceptionId = 1;

        public event PropertyChangedEventHandler PropertyChanged;
        public ExteriorLighting(
            string id,
            string projectId,
            string tag,
            string description,
            int typeId,
            int wattageDeterminedOptionId,
            int descriptionOptionId,
            float wattagePerLuminaire,
            float wattagePerLinearFoot,
            float totalLinearFeet,
            float numLuminaires,
            int mountingTypeId,
            bool excluded,
            bool moreThan6200Lumens,
            int luminaireShieldingExceptionId,
        )
        {
            this.id = id;
            this.projectId = projectId;
            this.tag = tag;
            this.description = description;
            this.typeId = typeId;
            this.wattageDeterminedOptionId = wattageDeterminedOptionId;
            this.descriptionOptionId = descriptionOptionId;
            this.wattagePerLuminaire = wattagePerLuminaire;
            this.wattagePerLinearFoot = wattagePerLinearFoot;
            this.totalLinearFeet = totalLinearFeet;
            this.numLuminaires = numLuminaires;
            this.mountingTypeId = mountingTypeId;
            this.excluded = excluded;
            this.moreThan6200Lumens = moreThan6200Lumens;
            this.luminaireShieldingExceptionId = luminaireShieldingExceptionId;
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
        public float WattagePerLuminaire
        {
            get => wattagePerLuminaire;
            set
            {
                if (wattagePerLuminaire != value)
                {
                    wattagePerLuminaire = value;
                    OnPropertyChanged(nameof(WattagePerLuminaire));
                }
            }
        }
        public float WattagePerLinearFoot
        {
            get => wattagePerLinearFoot;
            set
            {
                if (wattagePerLinearFoot != value)
                {
                    wattagePerLinearFoot = value;
                    OnPropertyChanged(nameof(WattagePerLinearFoot));
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
        public float NumLuminaires
        {
            get => numLuminaires;
            set
            {
                if (numLuminaires != value)
                {
                    numLuminaires = value;
                    OnPropertyChanged(nameof(NumLuminaires));
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


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
