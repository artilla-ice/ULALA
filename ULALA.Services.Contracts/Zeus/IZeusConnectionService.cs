
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Services.Contracts.Zeus.DTO.CashTotals;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;
using ULALA.Services.Contracts.Zeus.DTO.CashInsertion;
using ULALA.Services.Contracts.Events.MoneyInserted;

namespace ULALA.Services.Contracts.Zeus
{
    public interface IZeusConnectionService 
    {
        Task Initialize();
        bool StartListening();
        void StopComm();
        Task<Status> GetGeneralStatus();
        Task<CashTotalsResponse> RequestCashTotals();
        Task<MoneyRetrievalResponse> RetrieveStackerValues();
        Task RequestMoneyInsertion(double amountInsertion = -1);
        //Task<T> OnStartListeningForEvent<T>(string expectedResponse = "event");
        Task FinishMoneyInsertion();
        Task<bool> RequestDispenseSession(double amount);
        Task FinishDispenseSession();
        bool IsConnected { get; }
    }
}
