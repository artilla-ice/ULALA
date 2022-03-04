
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Infrastructure.Events;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;
using ULALA.Services.Contracts.Zeus;
using ULALA.Services.Contracts.Zeus.DTO.CashInsertion;
using ULALA.Services.Contracts.Zeus.DTO.CashRetrieval;
using ULALA.Services.Contracts.Zeus.DTO.Status;
using ULALA.UI.Core.Contracts.Navigation;
using Unity;
using Windows.UI.Core;

namespace ULALA.Core.Zeus
{
    public class ZeusManager : IZeusManager
    {
        [Dependency]
        public IZeusConnectionService ZeusConnectionService { get; set; }

        [Dependency]
        public INavigationManager NavigationManager { get; set; }

        private IEventAggregator EventAggregator { get; set; }

        public ZeusManager()
        {
        }

        public ZeusManager(IEventAggregator eventAggregator) 
        {
            this.EventAggregator = eventAggregator;
        }

        public Task Initialize()
        {
            SubscribeToEvents();

            return Task.CompletedTask;
        }

        public void OnStartListening()
        {
            this.ZeusConnectionService.StartListening();
        }

        public void OnCloseConnection()
        {
            this.ZeusConnectionService.StopComm();

        }

        public Task StartMoneyInsertion()
        {
            this.ZeusConnectionService.RequestMoneyInsertion();

            return Task.CompletedTask;
        }
        public async Task CloseMoneyInsertion()
        {
            IsInsertSessionOpen = false;

            await this.ZeusConnectionService.FinishMoneyInsertion();
        }

        public Task StartDispenseMoneySession(double amount)
        {
             this.ZeusConnectionService.RequestDispenseSession(amount);

            return Task.CompletedTask; ;
        }

        public async Task CloseDispenseSession()
        {
            IsDispenseSessionOpen = false;

            await this.ZeusConnectionService.FinishDispenseSession();
        }

        public async Task<MoneyRetrievalResponse> RetriveStackerCash()
        {
            return await this.ZeusConnectionService.RetrieveStackerValues();
        }

        public  IEnumerable<SystemInfoResultCode> GetErrors()
        {
            return  this.ZeusConnectionService.GetGeneralStatus().Result.Errors
                                            .Select(e => (SystemInfoResultCode)e.Code)
                                            .ToList();
        }

        public IEnumerable<SystemInfoResultCode> GetWarnings()
        {
            return this.ZeusConnectionService.GetGeneralStatus().Result.Warnings
                                            .Select(e => (SystemInfoResultCode)e.Code)
                                            .ToList();
        }

        public IEnumerable<WithdrawalCashModel> GetRecyclerValues()
        {
            var cashTotals = this.ZeusConnectionService.RequestCashTotals().Result;
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
            var cashTotals = this.ZeusConnectionService.RequestCashTotals().Result;
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

            var coinsValues = cashTotals.CashTotals.CashBoxInfo.CoinsInfo
                                    .Select(r => new WithdrawalStackerCashModel()
                                    {
                                        CashType = CashType.Coins,
                                        Denomination = r.Value,
                                        StackerQuantity = r.Count
                                    }).ToList();

            stackerValues.AddRange(coinsValues);

            return stackerValues;
        }

        private async void OnStartWithdrawalStackerView(MoneyRetrievalResponse args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var param = new Dictionary<string, object>() { { "MoneyRetrieval", args } };
                this.NavigationManager.NavigateTo(ViewNames.WithdrawStacker, param);
            });
        }

        private void SubscribeToEvents()
        {
            this.EventAggregator.GetEvent<MoneyRetrievalEvent>()
                .Subscribe((args) =>
                {
                    OnStartWithdrawalStackerView(args.Result);
                }, ThreadOption.UIThread);
        }

        public bool IsConnected => this.ZeusConnectionService.IsConnected;
        public bool IsInsertSessionOpen { get; set; }
        public  bool IsDispenseSessionOpen { get; set; }
    }
}
