using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infastructure
{
    public class Data
    {
        public string fullChartName { get; set; } = null;
        public string chartPartitionName { get; set; } = null;
        public string blockName { get; set; } = null;
        public string blockPinName { get; set; } = null;

        public PinConnectionType? pinConnectionType { get; set; } = null;

        public string externalVariableName { get; set; } = null;

        public bool? isBlockChart { get; set; } = null;

        public bool isParsingSuccess { get; set; } = false;

        public override string ToString()
        {
            return "fullChartName = " + fullChartName + "\n" +
                   "chartPartitionName = " + chartPartitionName + "\n" +
                   "blockName = " + blockName + "\n" +
                   "blockPinName = " + blockPinName + "\n" +
                   "pinConnectionType = " + pinConnectionType + "\n" +
                   "externalVariableName = " + externalVariableName + "\n" +
                   "isBlockChart = " + isBlockChart + "\n"+
                   "isParsingSuccess = " + isParsingSuccess + "\n\n";
        }
    }
}