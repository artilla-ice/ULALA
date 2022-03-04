using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Base;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Services.Contracts.Events.MoneyInserted;
using ULALA.Services.Contracts.Zeus.DTO.CashInsertion;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;

namespace ULALA.Core.Contracts.Zeus
{
    public interface IZeusManager : IInitializableManager
    {
        void OnStartListening();
        void OnCloseConnection();
        IEnumerable<WithdrawalCashModel> GetRecyclerValues();
        IEnumerable<WithdrawalStackerCashModel> GetStackerValues();
        IEnumerable<SystemInfoResultCode> GetErrors();
        IEnumerable<SystemInfoResultCode> GetWarnings();
        Task<MoneyRetrievalResponse> RetriveStackerCash();
        //Task<MoneyInsertedEvent> GetEventResponse();
        Task CloseMoneyInsertion(); 
        Task StartMoneyInsertion();
        Task StartDispenseMoneySession(double amount);
        Task CloseDispenseSession();
        bool IsConnected { get; }
        bool IsInsertSessionOpen { get; set; }

    }
}
