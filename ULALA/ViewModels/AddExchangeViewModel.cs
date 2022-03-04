using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Infrastructure.Events;
using ULALA.Infrastructure.PubSub;
using ULALA.Services.Contracts.Events.MoneyInserted;
using ULALA.UI.Core.MVVM;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;

namespace ULALA.ViewModels
{
    public class AddExchangeViewModel : ViewModelBase
    {
        [Unity.Dependency]
        public IZeusManager ZeusManager { get; set; }

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
        }

        private void OnStartMoneyInsertion()
        {
            HandleAsyncCall(async () =>
            {
                await this.ZeusManager.StartMoneyInsertion();
            });
        }

        private void OnFinalizeMoneyInsertion()
        {
            HandleAsyncCall(async () =>
            {
                await this.ZeusManager.CloseMoneyInsertion();
                this.ZeusManager.IsInsertSessionOpen = false;

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
                    Denomination = -1,
                    DenominationIconSize = 120
                },
            };

            amounts = amounts.OrderByDescending(a => a.Denomination).ToList();
            this.RecyclerAmounts = new ObservableCollection<FundsInfoModel>(amounts);
        }

        private void SubscribeToEvents()
        {
            this.EventAggregator.GetEvent<ResponseReceivedEvent>()
                .Subscribe((args) =>
                {
                    if (args.CommandId == "moneyInsertedEvent")
                        OnUpdateInsertedMoney((MoneyMovementEvent)args.Result);
                    else if (args.CommandId == "commandResponse")
                    {
                        var isValidResult = typeof(bool) == args.Result.GetType();
                        if(isValidResult && args.ResponseId == 1)
                            GetCommandResponse((bool)args.Result);
                    }

                }, ThreadOption.UIThread);
        }

        private void GetCommandResponse(bool result)
        {
            this.IsInserting = result;
            this.ZeusManager.IsInsertSessionOpen = IsInserting;

            ContentDialog dialog = new ContentDialog();
            if (IsInserting)
            {
                this.EventAggregator.GetEvent<StartListeningForResponseReceivedEvent>().Publish(new StartListeningForResponseReceivedEventArgs
                {
                    Response = "event",
                    EvenType = "moneyInsertedEvent"
                });

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
            }
            else
            {
                dialog.Title = new InfoBar()
                {
                    IsOpen = true,
                    IsIconVisible = true,
                    IsClosable = false,
                    Severity = InfoBarSeverity.Error,
                    Title = "Algo salió mal",
                    Message = "Comprueba la conexión con la caja",
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                    VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
                };
            }

            HandleAsyncCall(async () =>
            {
                dialog.PrimaryButtonText = "OK";

                await dialog.ShowAsync();
            });
        }

        private void OnUpdateInsertedMoney(MoneyMovementEvent movementEvent)
        {
            var currentInsertedMoney = movementEvent;
            if (currentInsertedMoney != null)
            {
                var denominationItem = this.RecyclerAmounts.Where(r => currentInsertedMoney.Type == "moneyInsertedEvent"
                                                    && currentInsertedMoney.Data[0].Value == r.Denomination).FirstOrDefault();

                var itemOnCollection = new FundsInfoModel();
                if (denominationItem == null)
                {
                    itemOnCollection = this.RecyclerAmounts.Where(r => r.Denomination == -1).FirstOrDefault();
                    itemOnCollection.RecyclerAmount += (uint)currentInsertedMoney.Data[0].Value;
                }
                else
                {
                    itemOnCollection = denominationItem;

                    itemOnCollection.RecyclerQuantity += (uint)currentInsertedMoney.Data.Count();
                }


                CalculateTotalAmount();
            }
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
