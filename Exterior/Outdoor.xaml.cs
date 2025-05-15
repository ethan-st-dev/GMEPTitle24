using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMEPTitle24.Exterior;

namespace GMEPTitle24
{
    /// <summary>
    /// Interaction logic for Outdoor.xaml
    /// </summary>
    public partial class Outdoor : UserControl
    {
        public OutdoorViewModel viewModel;
        public Outdoor(MainViewModel MainView)
        {
            viewModel = new OutdoorViewModel(MainView);
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
