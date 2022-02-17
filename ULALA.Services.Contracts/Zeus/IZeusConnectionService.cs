
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Services.Contracts.Zeus.DTO;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;

namespace ULALA.Services.Contracts.Zeus
{
    public interface IZeusConnectionService
    {
        bool StartListening();
        void StopComm();
        CashTotalsResponse RequestCashTotals();
        Task<MoneyRetrievalResponse> RetrieveStackerValues();
        bool IsConnected { get; }
    }
}
