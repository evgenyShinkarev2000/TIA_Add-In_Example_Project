using Infastructure;
using Siemens.Engineering;
using Siemens.Engineering.AddIn.Menu;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.FunctionCharts;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace TIA_Add_In_Example_Project
{
    public class AddIn : ContextMenuAddIn
    {
        TiaPortal _tiaPortal;

        private const string s_DisplayNameOfAddIn = "Add-In";
        private XMLViewLibrary.MainWindow mainWindow;
        private MenuSelectionProvider<DeviceItem> menuSelectionProvider;
        private const string logFileName = "MyLogs.log";

        public AddIn(TiaPortal tiaPortal) : base(s_DisplayNameOfAddIn)
        {
            File.AppendAllText(logFileName, "Constructor");
            _tiaPortal = tiaPortal;
        }

        private void UpdateButtonClickExternalHandler(object sender, EventArgs e)
        {
            if (menuSelectionProvider != null)
            {
                OnDoSomething(menuSelectionProvider);
            }
        }

        protected override void BuildContextMenuItems(ContextMenuAddInRoot menu)
        {
            menu.Items.AddActionItem<DeviceItem>(("Cross References For Charts"), OnDoSomething, OnCanSomething);
        }

        private void OnDoSomething(MenuSelectionProvider<DeviceItem> menuSelectionProvider)
        {

            if (menuSelectionProvider.GetSelection().First() is DeviceItem)
            {
                try
                {
                    File.AppendAllText(logFileName, "OnDoSomething begin");
                    //System.Diagnostics.Process.Start("C:\\Program Files\\Siemens\\Automation\\Portal V18\\ExternalApps\\TIA_Add-In_Example_Project\\XMLView\\bin\\Debug\\XMLView.exe");
                    //File.AppendAllText(logFileName, "Process run");
                    DeviceItem deviceItem = (DeviceItem)menuSelectionProvider.GetSelection().First();

                    PlcSoftware software = (PlcSoftware)deviceItem.GetService<SoftwareContainer>().Software;
                    ChartProviderS7 chartProvider = software.GetService<ChartProviderS7>();
                    string path = Path.GetTempPath() + "ChartsFrom" + deviceItem.Name;
                    chartProvider.CompleteExport(path, "V1.0", 0, true);
                    ZipArchive archive = ZipFile.OpenRead(path + ".zip");
                    ZipArchiveEntry entry = archive.GetEntry("Data.xml");
                    entry.ExtractToFile(path + ".xml", true);
                    MessageBox.Show(path);
                    //XDocument doc = XDocument.Load(entry.Open());
                    //MessageBox.Show(new string(doc.ToString().Take(300).ToArray()));
                    //var listData = Parser.Parse(doc);
                    //var v= listData.Take(5).Aggregate("",(string  concat, Data e) => $"{concat}{e.ToString()}");
                    //mainWindow.FlushData(listData);
                }
                catch (Exception e)
                {
                    File.AppendAllText(logFileName, "Got Exception\n" + e.ToString());
                    MessageBox.Show(e.Message + '\n' + e.StackTrace);
                }
            }
            else
            {
                MessageBox.Show("No device item in first selection");
            }

        }
        private MenuStatus OnCanSomething(MenuSelectionProvider<DeviceItem> menuSelectionProvider)
        {
            return MenuStatus.Enabled;
        }
    }
}
