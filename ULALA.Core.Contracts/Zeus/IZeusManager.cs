using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Core.Contracts.Zeus
{
    public interface IZeusManager
    {
        void OnStartListening();
        void OnCloseConnection();
        void GetCashTotals();
        bool IsConnected { get; }
    }
}
