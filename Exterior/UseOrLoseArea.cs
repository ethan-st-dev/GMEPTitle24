using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPTitle24.Exterior
{
    public class UseOrLoseArea: INotifyPropertyChanged
    {
        public string description = string.Empty;
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

        public int applicationTypeId = 1;
        public int ApplicationTypeId
        {
            get { return applicationTypeId; }
            set
            {
                if (applicationTypeId != value)
                {
                    applicationTypeId = value;
                    OnPropertyChanged(nameof(ApplicationTypeId));
                }
            }
        }
        public float area = 0;
        public float Area
        {
            get { return area; }
            set
            {
                if (area != value)
                {
                    area = value;
                    OnPropertyChanged(nameof(Area));
                }
            }
        }
        public UseOrLoseArea(string description, int applicationTypeId, float area)
        {
            this.description=description;
            this.applicationTypeId=applicationTypeId;
            this.area=area;
        }
        public UseOrLoseArea()
        {
            // :3
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
