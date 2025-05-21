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
        private string id = Guid.NewGuid().ToString();
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
        private string projectId = string.Empty;
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
        public int locationQty = 0;
        public int LocationQty
        {
            get { return locationQty; }
            set
            {
                if (locationQty != value)
                {
                    locationQty = value;
                    OnPropertyChanged(nameof(LocationQty));
                }
            }
        }
        public float linearFeet = 0;
        public float LinearFeet
        {
            get { return linearFeet; }
            set
            {
                if (linearFeet != value)
                {
                    linearFeet = value;
                    OnPropertyChanged(nameof(LinearFeet));
                }
            }
        }
        public UseOrLoseArea(string id, string projectId, string description, int applicationTypeId, float area, int locationQty, float linearFeet)
        {
            this.id = id;
            this.projectId = projectId;
            this.description=description;
            this.applicationTypeId=applicationTypeId;
            this.area=area;
            this.locationQty=locationQty;
            this.linearFeet=linearFeet;
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
