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
        public HardscapeArea(string description, float area)
        {
            this.description = description;
            this.area = area;
        }public HardscapeArea()
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
