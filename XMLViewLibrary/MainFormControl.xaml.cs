using Infastructure;
using Infrastructure;
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
        private readonly MainFormVM mainFormVM;
        private readonly XmlDataProvider xmlDataProvider = new XmlDataProvider();
        private IDataProvider dataProvider;
        private readonly ILogger logger;

        public MainFormControl()
        {
            InitializeComponent();
            this.logger = new TextBoxLogger(this.logAreaRichBox);
            mainFormVM = new MainFormVM(logger);
            DataContext = mainFormVM;
            dataProvider = xmlDataProvider;
            dataProvider.DataUpdated += FlushData;
            dataProvider.Failed += logger.LogError;
        }

        public void FlushData(IEnumerable<Data> dataRecords)
        {
            mainFormVM.FlushData(dataRecords);
        }

        private void OpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "XML Data file|*.xml";
            if (dialog.ShowDialog() == true)
            {
                pathToSourceXml = dialog.FileName;
                xmlDataProvider.WatchXmlFile(pathToSourceXml);
                xmlDataProvider.ForceUpdate();
            }
        }

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            xmlDataProvider.ForceUpdate();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            this.mainFormVM.SearchWord = this.mainFormVM.SearchWord;
        }
    }
}
