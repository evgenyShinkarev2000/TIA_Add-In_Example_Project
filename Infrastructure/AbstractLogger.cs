using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public abstract class AbstractLogger : ILogger
    {
        public virtual void LogWithStatus(object stringConvertable, LogEventStatus eventStatus)
        {
            switch (eventStatus)
            {
                case LogEventStatus.Action:
                    LogAction(stringConvertable);
                    break;
                case LogEventStatus.ControlPointAction:
                    LogControlPointAction(stringConvertable);
                    break;
                case LogEventStatus.Warning:
                    LogWarning(stringConvertable);
                    break;
                case LogEventStatus.CriticalWarning:
                    LogCritical(stringConvertable);
                    break;
                case LogEventStatus.Error:
                    LogError(stringConvertable);
                    break;
            }
        }

        public abstract void LogAction(object stringConvertable);

        public abstract void LogControlPointAction(object stringConvertabe);

        public abstract void LogCritical(object stringConvertable);

        public abstract void LogError(object stringConvertable);

        public abstract void LogWarning(object stringConvertable);
    }
}
