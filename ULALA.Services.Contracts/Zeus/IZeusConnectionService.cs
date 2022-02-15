
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus
{
    public interface IZeusConnectionService
    {
        bool StartListening();
        void StopComm();
        void RequestCashTotals();
        bool IsConnected { get; }
    }
}
