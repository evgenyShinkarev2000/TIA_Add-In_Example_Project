using Infastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace XMLViewLibrary
{
    internal class DataVM
    {
        private readonly Infastructure.Data data;

        public DataVM(Infastructure.Data data)
        {
            this.data = data;
        }
        [ViewName("Variable Name")]
        public string VariableName => data.externalVariableName;
        [ViewName("Chart Name")]
        public string ChartName => data.fullChartName;
        [ViewName("Chart")]
        public string ChartPartitionName => data.chartPartitionName;
        [ViewName("Block Name")]
        public string BlockName => data.blockName;
        [ViewName("Pin Name")]
        public string PinName => data.blockPinName;
        [ViewName("Pin Type")]
        public string PinType => getPinTypeName(data.pinConnectionType );

        private string getPinTypeName(PinConnectionType? pinConnectionType)
            => pinConnectionType != null
            ? Enum.GetName(typeof(PinConnectionType), pinConnectionType)
            : "no data";

    }
}
