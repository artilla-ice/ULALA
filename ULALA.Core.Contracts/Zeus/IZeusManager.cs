using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus.DTO;

namespace ULALA.Core.Contracts.Zeus
{
    public interface IZeusManager
    {
        void OnStartListening();
        void OnCloseConnection();
        IEnumerable<WithdrawalCashModel> GetRecyclerValues();
        bool IsConnected { get; }
    }
}
