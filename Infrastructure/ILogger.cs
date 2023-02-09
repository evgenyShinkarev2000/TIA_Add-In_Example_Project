using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface ILogger
    {
        void LogAction(object stringConvertable);
        void LogControlPointAction(object stringConvertabe);
        void LogWarning(object stringConvertable);
        void LogCritical(object stringConvertable);
        void LogError(object stringConvertable);
        void LogWithStatus(object stringConvertable, LogEventStatus eventStatus);
    }
}
