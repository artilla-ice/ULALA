﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Services.Contracts.Events.MoneyInserted;
using ULALA.Services.Contracts.Zeus.DTO.CashInsertion;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;

namespace ULALA.Core.Contracts.Zeus
{
    public interface IZeusManager
    {
        void OnStartListening();
        void OnCloseConnection();
        IEnumerable<WithdrawalCashModel> GetRecyclerValues();
        IEnumerable<WithdrawalStackerCashModel> GetStackerValues();
        IEnumerable<SystemInfoResultCode> GetErrors();
        IEnumerable<SystemInfoResultCode> GetWarnings();
        Task<MoneyRetrievalResponse> RetriveStackerCash();
        Task<MoneyInsertedEvent> GetEventResponse();
        Task<FinishInsertionResponse> CloseMoneyInsertion();
        bool StartMoneyInsertion();
        bool IsConnected { get; }
    }
}
