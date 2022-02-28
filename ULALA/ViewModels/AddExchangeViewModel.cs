using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ULALA.Core.Contracts.Events;
using ULALA.Core.Contracts.Logger;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Infrastructure.PubSub;
using ULALA.UI.Core.MVVM;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;

namespace ULALA.ViewModels
{
    public class AddExchangeViewModel : ViewModelBase
    {
        [Unity.Dependency]
        public IZeusManager ZeusManager { get; set; }

        [Unity.Dependency]
        public ILogger Logger { get; set; }

        public Command StartInsertionCommand { get; }
        public Command EndInsertionCommand { get; }

        public AddExchangeViewModel()
        {
        }

        public AddExchangeViewModel(IEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;

            this.StartInsertionCommand = new Command(OnStartMoneyInsertion);
            this.EndInsertionCommand = new Command(OnFinalizeMoneyInsertion);

            SubscribeToEvents();
        }

        protected override void OnActivated()
        {
            OnLoadAllDenominationsInfo();

            this.Logger.WriteEvent();
        }

        private void OnStartMoneyInsertion()
        {
            var isReadyForInsertion = this.ZeusManager.StartMoneyInsertion();
            this.IsInserting = isReadyForInsertion;

            HandleAsyncCall(async () =>
            {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = new InfoBar()
                {
                    IsOpen = true,
                    IsIconVisible = true,
                    IsClosable = false,
                    Severity = InfoBarSeverity.Informational,
                    Title = "Ya puedes ingresar cambio",
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                    VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
                };

                dialog.PrimaryButtonText = "OK";

                await dialog.ShowAsync();
            });
        }

        private void OnFinalizeMoneyInsertion()
        {
            HandleAsyncCall(async () =>
            {
                await this.ZeusManager.CloseMoneyInsertion();

                this.IsInserting = false;

                ContentDialog dialog = new ContentDialog();
                dialog.Title = new InfoBar()
                {
                    IsOpen = true,
                    IsIconVisible = true,
                    IsClosable = false,
                    Severity = InfoBarSeverity.Success,
                    Title = "Cambio agregado con éxito",
                    Message = "El dinero ha sido agregado al reciclador",
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                    VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
                };

                dialog.PrimaryButtonText = "OK";

                await dialog.ShowAsync();
            });        
        }

        private void OnLoadAllDenominationsInfo()
        {
            var amounts = new List<RecyclerToCashierFundsModel>()
            {
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 1000
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 500
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 200
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 100
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 50
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 20
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Coins,
                    Denomination = 10
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Coins,
                    Denomination = 5
                },
                 new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Coins,
                    Denomination = 2
                },
                  new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Coins,
                    Denomination = 1
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Coins,
                    Denomination = .50
                }
            };

            amounts = amounts.OrderByDescending(a => a.Denomination).ToList();
            this.RecyclerAmounts = new ObservableCollection<FundsInfoModel>(amounts);
        }

        private void SubscribeToEvents()
        {
            this.EventAggregator.GetEvent<NewMoneyInsertEvent>()
                .Subscribe((args) =>
               {
                   var currentInsertedMoney = args.Response;
                   if (currentInsertedMoney != null)
                   {
                       var itemOnCollection = this.RecyclerAmounts.Where(r => currentInsertedMoney.Type == "moneyInsertedEvent"
                                                       && currentInsertedMoney.Data[0] != null
                                                       && currentInsertedMoney.Data[0].Value == r.Denomination).FirstOrDefault();

                       itemOnCollection.RecyclerQuantity += (uint)currentInsertedMoney.Data.Count();

                       CalculateTotalAmount();
                   }
               }, ThreadOption.UIThread);
        }

        private void CalculateTotalAmount()
        {
            this.RecyclerTotalAmount = m_recyclerAmounts.Sum(a => a.RecyclerAmount);
        }

        private ObservableCollection<FundsInfoModel> m_recyclerAmounts;
        public ObservableCollection<FundsInfoModel> RecyclerAmounts
        {
            get { return m_recyclerAmounts; }
            set { SetProperty(ref m_recyclerAmounts, value); }
        }

        private bool m_isInserting = false;
        public bool IsInserting
        {
            get { return m_isInserting; }
            set { SetProperty(ref m_isInserting, value); }
        }

        private double m_recyclerTotalAmount;
        public double RecyclerTotalAmount
        {
            get { return m_recyclerTotalAmount; }
            set { SetProperty(ref m_recyclerTotalAmount, value); }
        }

        private IEventAggregator EventAggregator { get; set; }
    }
}
