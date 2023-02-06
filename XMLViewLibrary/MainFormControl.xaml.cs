using Infastructure;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace XMLViewLibrary
{
    /// <summary>
    /// Interaction logic for MainFormControl.xaml
    /// </summary>
    public partial class MainFormControl : UserControl
    {
        private IEnumerable<Data> dataRecords = Enumerable.Empty<Data>();
        public event EventHandler UpdateButtonClickNotify;

        public MainFormControl()
        {
            InitializeComponent();
        }

        public void FlushData(IEnumerable<Data> dataRecords)
        {
            if (dataRecords == null)
            {
                throw new ArgumentNullException();
            }
            this.dataRecords = dataRecords;
            UpdateFilteredData();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            var searchWord = this.searchTextBox.Text?.Trim();
            UpdateFilteredData(searchWord);
        }

        private void UpdateDataGrid(IEnumerable<Data> updatedData)
        {
            this.recordsDataGrid.ItemsSource = updatedData.Select(d => new DataVM(d));
            this.foundCountLabel.Content = $"{this.recordsDataGrid.Items.Count} found";
        }

        private void HighlightFound(Color color)
        {
            var brush = new SolidColorBrush(color);
            foreach (var i in Enumerable.Range(0, VisualTreeHelper.GetChildrenCount(this.recordsDataGrid)))
            {
                var child = VisualTreeHelper.GetChild(this.recordsDataGrid, i);
            }

        }

        private void UpdateFilteredData(string searchWord = "")
        {
            if (searchWord == "" || searchWord == null)
            {
                UpdateDataGrid(dataRecords);
            }
            else
            {
                UpdateDataGrid(dataRecords.Where(d => d.externalVariableName.Contains(searchWord)));
                HighlightFound(Colors.Yellow);
            }
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchWord = this.searchTextBox.Text?.Trim();
            UpdateFilteredData(searchWord);
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            this.UpdateButtonClickNotify.Invoke(sender, e);
        }
    }
}
