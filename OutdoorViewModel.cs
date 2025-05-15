using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GMEPTitle24
{
    public class OutdoorViewModel: INotifyPropertyChanged
    {
        public MainViewModel MainView { get; set; }
        public OutdoorViewModel(MainViewModel MainView)
        {
            this.MainView = MainView;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
