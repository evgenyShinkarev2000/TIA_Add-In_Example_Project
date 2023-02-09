using Infastructure;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XMLViewLibrary
{
    internal class MainFormVM : INotifyPropertyChangedFromAnywhere
    {
        [DependentProperty(nameof(SelectedDataSource))]
        public Visibility FileInputPannelVisible
        {
            get => SelectedDataSource == XMLViewLibrary.DataSources.XMLFile 
                ? Visibility.Visible 
                : Visibility.Hidden;
        }
        public IEnumerable<ComboBoxItemVM<DataSources>> DataSources
        {
            get => new[] {
                new ComboBoxItemVM<DataSources>(XMLViewLibrary.DataSources.NamedPipe, "Named pipe"),
                new ComboBoxItemVM<DataSources>(XMLViewLibrary.DataSources.XMLFile, "XML file"),
            };
        }
        /// <summary>
        /// OneWayToSource Binding
        /// </summary>
        private DataSources selectedDataSource;
        /// <summary>
        /// OneWayToSource Binding
        /// </summary>
        public DataSources SelectedDataSource 
        {
            get => selectedDataSource;
            set
            {
                selectedDataSource = value;
                NotifyPropertyChanged();
            }
        }

        private IEnumerable<DataVM> dataGridRecords = Enumerable.Empty<DataVM>();
        [DependentProperty(nameof(SearchWord))]
        public IEnumerable<DataVM> DataGridFilteredRecords 
        {
            get => searchWord == ""
                ? dataGridRecords
                : dataGridRecords.Where(d => d.VariableName.Contains(searchWord));
            private set
            {
                dataGridRecords = value;
                NotifyPropertyChanged();
            }
        }

        private string searchWord = "";
        public string SearchWord
        {
            get => searchWord;
            set
            {
                if (value == null || value.Trim().Length == 0)
                {
                    searchWord = "";
                }
                else
                {
                    searchWord = value.Trim();
                }
                NotifyPropertyChanged();
            }
        }

        [DependentProperty(nameof(DataGridFilteredRecords))]
        public string ItemsFound
        {
            get => $"Found {DataGridFilteredRecords.Count()}";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly ILogger logger;
        private readonly PropertyWatcher propertyWatcher;

        public MainFormVM(ILogger logger)
        {
            this.logger = logger;
            propertyWatcher = new PropertyWatcher(this);
        }
        
        public void FlushData(IEnumerable<Data> records)
        {
            var recordsVM = DataGridFilteredRecords = records.Select(r => new DataVM(r));
            DataGridFilteredRecords = recordsVM;
        }

        public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
