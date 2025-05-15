using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using GMEPTitle24.Interior;

namespace GMEPTitle24
{
    /// <summary>
    /// Interaction logic for Indoor.xaml
    /// </summary>
    public partial class Indoor : UserControl
    {
        public IndoorViewModel viewModel;
        public Indoor(MainViewModel mainview)
        {
            viewModel = new IndoorViewModel(mainview);
            InitializeComponent();
            this.DataContext = viewModel;

            viewModel.ResetColumns += ViewModel_ResetColumns;
            viewModel.ResetRows += ViewModel_ResetRows;
        }

        private void ViewModel_ResetRows(object? sender, EventArgs e)
        {
            Row1.ClearValue(RowDefinition.HeightProperty);
            Row3.ClearValue(RowDefinition.HeightProperty);
        }

        private void ViewModel_ResetColumns(object? sender, EventArgs e)
        {
            Column1.ClearValue(ColumnDefinition.WidthProperty);
            Column3.ClearValue(ColumnDefinition.WidthProperty);
        }

        public void OptionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OptionsGrid.SelectedItem is Lighting selectedLighting)
            {
                AdditionalOptionsGrid.SelectedItem = selectedLighting;
                AdditionalOptionsGrid.ScrollIntoView(selectedLighting);
            }
        }

        public void AdditionalOptionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AdditionalOptionsGrid.SelectedItem is Lighting selectedLighting)
            {
                OptionsGrid.SelectedItem = selectedLighting;
                OptionsGrid.ScrollIntoView(selectedLighting);
            }
        }




    }
}
