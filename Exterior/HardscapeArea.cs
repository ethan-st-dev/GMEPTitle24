using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPTitle24.Exterior
{
    public class HardscapeArea: INotifyPropertyChanged
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
        public float perimeterLength = 0;
        public float PerimeterLength
        {
            get { return perimeterLength; }
            set
            {
                if (perimeterLength != value)
                {
                    perimeterLength = value;
                    OnPropertyChanged(nameof(PerimeterLength));
                }
            }
        }
        public HardscapeArea(string id, string projectId, string description, float area, float perimeterLength)
        {
            this.description = description;
            this.area = area;
            this.id = id;
            this.projectId = projectId;
            this.perimeterLength=perimeterLength;
        }
        public HardscapeArea()
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
