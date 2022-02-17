﻿
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Services.Contracts.Events.MoneyInserted;
using ULALA.Services.Contracts.Zeus;
using ULALA.Services.Contracts.Zeus.DTO.CashInsertion;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;
using Unity;

namespace ULALA.Core.Zeus
{
    public class ZeusManager : IZeusManager
    {
        [Dependency]
        public IZeusConnectionService ZeusConnectionService { get; set; }

        public ZeusManager() 
        { }

        public void OnStartListening()
        {
            this.ZeusConnectionService.StartListening();
        }

        public void OnCloseConnection()
        {
            this.ZeusConnectionService.StopComm();
        }

        public bool StartMoneyInsertion()
        {
            return this.ZeusConnectionService.RequestMoneyInsertion();
        }

        public async Task<FinishInsertionResponse> CloseMoneyInsertion()
        {
            return await this.ZeusConnectionService.FinishMoneyInsertion();
        }

        public async Task<MoneyInsertedEvent> GetEventResponse()
        {
            return await this.ZeusConnectionService.OnStartListeningForEvent<MoneyInsertedEvent>("event");
        }

        public async Task<MoneyRetrievalResponse> RetriveStackerCash()
        {
            return await this.ZeusConnectionService.RetrieveStackerValues();
        }

        public IEnumerable<SystemInfoResultCode> GetErrors()
        {
            return this.ZeusConnectionService.GetGeneralStatus().Errors
                                            .Select(e => (SystemInfoResultCode)e.Code)
                                            .ToList();
        }

        public IEnumerable<SystemInfoResultCode> GetWarnings()
        {
            return this.ZeusConnectionService.GetGeneralStatus().Warnings
                                            .Select(e => (SystemInfoResultCode)e.Code)
                                            .ToList();
        }

        public IEnumerable<WithdrawalCashModel> GetRecyclerValues()
        {
            var cashTotals = this.ZeusConnectionService.RequestCashTotals();
            if (!ZeusConnectionService.IsConnected)
                return null;

            if (cashTotals == null || cashTotals.CashTotals.RecyclersInfo == null)
                return null;

            var recyclerValues = cashTotals.CashTotals.RecyclersInfo.RecyclerBills
                                    .Select(r => new WithdrawalCashModel()
                                    {
                                        CashType = CashType.Bills,
                                        Denomination = r.Value,
                                        RecyclerQuantity = r.Count
                                    }).ToList();

            recyclerValues.AddRange(cashTotals.CashTotals.HoppersInfo.CoinsInfo
                                    .Select(r => new WithdrawalCashModel()
                                    {
                                        CashType = CashType.Coins,
                                        Denomination = r.Value,
                                        RecyclerQuantity = r.Count
                                    }).ToList());

            return recyclerValues;
        }

        public IEnumerable<WithdrawalStackerCashModel> GetStackerValues()
        {
            var cashTotals = this.ZeusConnectionService.RequestCashTotals();
            if (!ZeusConnectionService.IsConnected)
                return null;

            if (cashTotals == null || cashTotals.CashTotals.StackerInfo == null)
                return null;

            var stackerValues = cashTotals.CashTotals.StackerInfo.BillsInfo
                                    .Select(r => new WithdrawalStackerCashModel()
                                    {
                                        CashType = CashType.Bills,
                                        Denomination = r.Value,
                                        StackerQuantity = r.Count
                                    }).ToList();

            return stackerValues;
        }
        public bool IsConnected => this.ZeusConnectionService.IsConnected;
    }
}
