
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
           // SetTimer();

            SubscribeToEvents();

            //this.EventAggregator.GetEvent<MoneyRetrievalEvent>().Publish(new MoneyRetrievalEventEventArgs());

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


        public bool StartMoneyInsertion()
        {
            m_isInsertSessionOpen = this.ZeusConnectionService.RequestMoneyInsertion().Result;
              //this.EventAggregator.GetEvent<ReceivedResponseEvent>()
              //  .Subscribe((args) =>
              //  {
              //      m_isInsertSessionOpen = (bool)args.Response;
              //  }, ThreadOption.BackgroundThread);

            if (m_isInsertSessionOpen)
                this.EventAggregator.GetEvent<StartMoneyInsertionEvent>().Publish(new EventArgs());

            return m_isInsertSessionOpen;
        }
        public async Task CloseMoneyInsertion()
        {
            m_isInsertSessionOpen = false;

            await this.ZeusConnectionService.FinishMoneyInsertion();
        }

        public bool StartDispenseMoneySession(double amount)
        {
            m_isDispenseSessionOpen = this.ZeusConnectionService.RequestDispenseSession(amount).Result;

            if (m_isDispenseSessionOpen)
                this.EventAggregator.GetEvent<StartDispenseMoneySessionEvent>().Publish(new EventArgs());

            return m_isDispenseSessionOpen;
        }

        public async Task CloseDispenseSession()
        {
            m_isDispenseSessionOpen = false;

            await this.ZeusConnectionService.FinishDispenseSession();
        }


        //public async Task<MoneyInsertedEvent> GetEventResponse()
        //{
        //    return await this.ZeusConnectionService.OnStartListeningForEvent<MoneyInsertedEvent>("event");
        //}

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

        private void SubscribeToEvents()
        {
            this.EventAggregator.GetEvent<StartMoneyInsertionEvent>()
                .Subscribe(async (args) =>
                {
                    while(m_isInsertSessionOpen)
                    {
                        var result = await this.ZeusConnectionService.OnStartListeningForEvent<MoneyMovementEvent>();
                        if (result != null && result.Type == "moneyInsertedEvent")
                        {
                            this.EventAggregator.GetEvent<NewMoneyInsertEvent>().Publish(new NewMoneyInsertEventArgs
                            {
                                Response = result
                            });
                        }
                    }
                }, ThreadOption.BackgroundThread);

            this.EventAggregator.GetEvent<StartDispenseMoneySessionEvent>()
                .Subscribe(async (args) =>
                {
                    while (m_isDispenseSessionOpen)
                    {
                        var result = await this.ZeusConnectionService.OnStartListeningForEvent<MoneyMovementEvent>();
                        if(result != null && result.Type == "moneyDispensedEvent")
                        {
                            this.EventAggregator.GetEvent<NewMoneyDispensedEvent>().Publish(new NewMoneyDispensedEventArgs
                            {
                                Response = result
                            });
                        }
                    }
                }, ThreadOption.BackgroundThread);

            //this.EventAggregator.GetEvent<MoneyRetrievalEvent>()
            //   .Subscribe( (args) =>
            //   {
            //       SetTimer();
            //   }, ThreadOption.PublisherThread);
        }

        private async void OnStartWithdrawalStackerView(MoneyRetrievalResponse args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var param = new Dictionary<string, object>() { { "MoneyRetrieval", args } };
                this.NavigationManager.NavigateTo(ViewNames.WithdrawStacker, param);
                //m_timer.Change(0, 500);
            });
        }

        private async void StartListening(object state)
        {
            if (!IsConnected)
                return;

            m_timer.Change(Timeout.Infinite, Timeout.Infinite);
            var result = await this.ZeusConnectionService.OnStartListeningForEvent<MoneyRetrievalResponse>();
            if (result != null && result.Type == "moneyRetrieval")
            {
                OnStartWithdrawalStackerView(result);
            }
            m_timer.Change(2000, 2000);
        }

        private void SetTimer()
        {
            // Create a timer with a two second interval.
            m_timer = new Timer(StartListening, null, 0, 2000);
        }

        private static Timer m_timer;

        public bool IsConnected => this.ZeusConnectionService.IsConnected;

        private bool m_isInsertSessionOpen = false;
        private bool m_isDispenseSessionOpen = false;
    }
}
