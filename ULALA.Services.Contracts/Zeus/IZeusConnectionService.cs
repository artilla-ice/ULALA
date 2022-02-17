﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Services.Contracts.Zeus.DTO.CashTotals;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;

namespace ULALA.Services.Contracts.Zeus
{
    public interface IZeusConnectionService
    {
        bool StartListening();
        void StopComm();
        Status GetGeneralStatus();
        CashTotalsResponse RequestCashTotals();
        Task<MoneyRetrievalResponse> RetrieveStackerValues();
        bool IsConnected { get; }
    }
}
