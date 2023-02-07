using Infastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using XMLViewLibrary;

namespace XMLView
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal partial class MainWindow : Window
    {
        private readonly string pathToMockXML = System.IO.Path.Combine(
                new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.FullName,
                "Data.xml");

        private void UpdateButtonClickHandler(object sender, EventArgs e) => FlushMockData();
        public MainWindow()
        {
            InitializeComponent();
            //FlushMockData();
        }

        private void FlushMockData()
        {
            using (var fileXmlStream = File.OpenRead(pathToMockXML))
            {
                var data = Parser.Parse(XDocument.Load(fileXmlStream));
                this.mainFormControl.FlushData(data);
            }
        }
    }
}
