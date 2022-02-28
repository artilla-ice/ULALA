using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Core.Contracts.Logger
{
    public enum LogLevel
    {
        Information = 0x000,
        Warning     = 0x001,
        Error       = 0x002,
        Debug       = 0x004
    }

    public interface ILogger
    {
        void WriteEvent();
        void WriteError();
        void WriteWarning();
    }
}
