using Infastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using System.Xml.Linq;

namespace XMLViewLibrary
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public event EventHandler UpdateButtonClickNotify;
        private void UpdateButtonClickInnerHandler(object sender, EventArgs e) =>
            UpdateButtonClickNotify.Invoke(sender, e);
        public MainWindow()
        {
            InitializeComponent();
        }

        public void FlushData(IEnumerable<Data> dataRecords)
            => this.mainFormControl.FlushData(dataRecords);
    }
}
