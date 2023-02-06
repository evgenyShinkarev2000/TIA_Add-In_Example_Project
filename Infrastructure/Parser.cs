using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Infastructure
{
    public class Parser
    {
        static public IEnumerable<Data> Parse(XDocument xDoc)
        {
            XNamespace ns = xDoc.Root.Name.Namespace;
            IEnumerable<XElement> externalVariables = xDoc.Descendants(ns + "ConnectionType").Where(e => e.Value == "ExternalVariable");
            List<Data> allData = new List<Data>();
            foreach (XElement extVar in externalVariables)
            {
                try
                {
                    Data data = new Data();

                    XElement attrList = extVar.Parent;

                    //connection type
                    IEnumerable<string> tagNames = attrList.Elements().Select(selected => selected.Name.LocalName);
                    if (tagNames.Contains("Source") && !tagNames.Contains("Sink")) data.pinConnectionType = PinConnectionType.SOURCE;
                    else if (!tagNames.Contains("Source") && tagNames.Contains("Sink")) data.pinConnectionType = PinConnectionType.SINK;

                    //external variable name
                    IEnumerable<XElement> sourceOrSink = attrList.Elements().Where(e => e.Name.LocalName == "Source" || e.Name.LocalName == "Sink");
                    if (sourceOrSink.Count() == 1) data.externalVariableName = sourceOrSink.First().Value;

                    //
                    IEnumerable<XElement> varMembers = extVar.Ancestors().Where(e => e.Name.LocalName == "VariableMember");
                    //block
                    if (varMembers.Count() > 0)
                    {
                        XAttribute pinName = varMembers.First().Attribute("Name");
                        data.blockPinName = (string)pinName;
                        data.isBlockChart = false;
                        IEnumerable<XElement> varRoot = extVar.Ancestors().Where(e => e.Name.LocalName == "VariableRoot");
                        if (varRoot.Count() > 0)
                        {
                            XAttribute blockName = varRoot.First().Attribute("Name");
                            data.blockName = (string)blockName;
                            IEnumerable<XElement> prog = extVar.Ancestors().Where(e => e.Name.LocalName == "Program").Reverse();
                            string fullChartName = "";
                            foreach (XElement x in prog)
                            {
                                fullChartName += x.Attribute("Name").Value + @"\";
                            }
                            if (fullChartName.Length > 0) data.fullChartName = fullChartName;
                            data.chartPartitionName = varRoot.First().Element(ns + "AttributeList").Element(ns + "Partition").Value;

                        }
                    }
                    //chart
                    else
                    {
                        IEnumerable<XElement> ioVars = extVar.Ancestors().Where(e => e.Name.LocalName == "IOVariable");
                        if (ioVars.Count() > 0)
                        {
                            XAttribute pinName = ioVars.First().Attribute("Name");
                            data.blockPinName = (string)pinName;
                            data.isBlockChart = true;
                            IEnumerable<XElement> prog = extVar.Ancestors().Where(e => e.Name.LocalName == "Program");
                            if (prog.Count() > 0)
                            {
                                XAttribute blockName = prog.First().Attribute("Name");
                                data.blockName = (string)blockName;

                                data.chartPartitionName = prog.First().Element(ns + "AttributeList").Element(ns + "Partition").Value;

                                prog = prog.Reverse();
                                string fullChartName = "";
                                for (int i = 0; i < prog.Count() - 1; i++)
                                {
                                    fullChartName += prog.ElementAt(i).Attribute("Name").Value + @"\";
                                }
                                data.fullChartName = fullChartName;
                            }
                        }
                    }

                    allData.Add(data);
                }
                catch (Exception e) { }
            }
            return allData.Select<Data, Data>(e =>
            {
                if (e.fullChartName == null || e.chartPartitionName == null || e.blockName == null || e.blockPinName == null || e.pinConnectionType == null || e.externalVariableName == null || e.isBlockChart == null)
                {
                    e.isParsingSuccess = false;
                }
                else
                {
                    e.isParsingSuccess = true;
                }
                return e;
            }
            ); 
        }
    }
}
