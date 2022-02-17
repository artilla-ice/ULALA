using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;

namespace ULALA.Core.Contracts.Zeus
{
    public interface IZeusManager
    {
        void OnStartListening();
        void OnCloseConnection();
        IEnumerable<WithdrawalCashModel> GetRecyclerValues();
        IEnumerable<WithdrawalStackerCashModel> GetStackerValues();
        Task<MoneyRetrievalResponse> RetriveStackerCash();
        bool IsConnected { get; }
    }
}
