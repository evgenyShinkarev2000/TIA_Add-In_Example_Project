using Infastructure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Xml.Linq;

namespace XMLViewLibrary
{
    /// <summary>
    /// Interaction logic for MainFormControl.xaml
    /// </summary>
    public partial class MainFormControl : UserControl
    {
        private IEnumerable<Data> dataRecords = Enumerable.Empty<Data>();
        private string pathToSourceXml;
        private readonly FileSystemWatcher fileWatcher = new FileSystemWatcher();

        public MainFormControl()
        {
            InitializeComponent();
            fileWatcher.Changed += SourceFileChangeHandler;
        }

        ~MainFormControl()
        {
            fileWatcher.Dispose();
        }

        public void FlushData(IEnumerable<Data> dataRecords)
        {
            if (dataRecords == null)
            {
                throw new ArgumentNullException();
            }
            this.dataRecords = dataRecords;
            this.updatedAtLabel.Content = $"Updated at {DateTime.Now.ToLongTimeString()}";
            this.updatedAtLabel.Foreground = Brushes.Black;
            this.outdatedLabel.Visibility = Visibility.Hidden;
            UpdateFilteredData();
        }

        private void SourceFileChangeHandler(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                this.updatedAtLabel.Foreground = new SolidColorBrush(Colors.OrangeRed);
                this.outdatedLabel.Visibility = Visibility.Visible;
                if (this.autoUpdateCheckBox.IsChecked == true)
                {
                    FlushDataFromXML();
                }
            });
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

        private void UpdateFilteredData(string searchWord = "")
        {
            if (searchWord == "" || searchWord == null)
            {
                UpdateDataGrid(dataRecords);
            }
            else
            {
                UpdateDataGrid(dataRecords.Where(d => d.externalVariableName.Contains(searchWord)));
            }
        }

        private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchWord = this.searchTextBox.Text?.Trim();
            UpdateFilteredData(searchWord);
        }

        private void OpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "XML Data file|*.xml";
            if (dialog.ShowDialog() == true)
            {
                pathToSourceXml = dialog.FileName;
                fileWatcher.Path = System.IO.Path.GetDirectoryName(pathToSourceXml);
                fileWatcher.Filter = System.IO.Path.GetFileName(pathToSourceXml);
                fileWatcher.EnableRaisingEvents = true;
                this.updateFromXmlButton.IsEnabled = true;
                FlushDataFromXML();
            }
        }

        private IEnumerable<Data> GetDataFromXml()
        {
            Thread.Sleep(500);
            using(var fileStream = File.OpenRead(pathToSourceXml))
            {
                var document = XDocument.Load(fileStream);

                return Parser.Parse(document);
            }
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            FlushDataFromXML();
        }

        private void FlushDataFromXML()
        {
            var dataRecords = GetDataFromXml();
            FlushData(dataRecords);
        }
    }
}
