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
using ULALA.Services.Contracts.Zeus.DTO.CashDispension;
using ULALA.UI.Core.MVVM;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;

namespace ULALA.ViewModels
{
    [QueryProperty(nameof(TotalChargeAmount), nameof(TotalChargeAmount))]
    public class NewChargeViewModel : ViewModelBase
    {
        [Unity.Dependency]
        public IZeusManager ZeusManager { get; set; }

        public Command StartInsertionCommand { get; }
        public Command EndInsertionCommand { get; }

        public NewChargeViewModel()
        {
        }

        public NewChargeViewModel(IEventAggregator eventAggregator)
        {
            this.EventAggregator = eventAggregator;

            this.StartInsertionCommand = new Command(OnStartMoneyInsertion);
            this.EndInsertionCommand = new Command(OnFinalizeMoneyInsertion);
        }

        protected override void OnActivated()
        {
            this.PageIcon = "../Assets/Icons/Cobrar.png"; //TODO: agregar path de iconos a archivo de recursos

            SubscribeToEvents();
            OnStartMoneyInsertion();
        }

        protected override void OnDeactivated()
        {
            this.EventAggregator.GetEvent<ResponseReceivedEvent>()
                .Unsubscribe((args) =>
                {
                    HandleResponse(args);
                });
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

                this.IsInserting = false;

                await this.ZeusManager.StartDispenseMoneySession(this.ExchangeAmount);

                ContentDialog dialog = new ContentDialog();
                dialog.Title = new InfoBar()
                {
                    IsOpen = true,
                    IsIconVisible = true,
                    IsClosable = false,
                    Severity = InfoBarSeverity.Success,
                    Title = "Cambio dispensado con éxito",
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                    VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
                };

                dialog.PrimaryButtonText = "OK";

                await dialog.ShowAsync();
            });
        }

        private void SubscribeToEvents()
        {
            this.EventAggregator.GetEvent<ResponseReceivedEvent>()
                .Subscribe((args) =>
                {
                    HandleResponse(args);
                }, ThreadOption.UIThread);
        }

        private void HandleResponse(ResponseReceivedEventArgs args = null)
        {
            if (args.CommandId == "moneyInsertedEvent")
                OnUpdateInsertedMoney((MoneyMovementEvent)args.Result);
            else if (args.CommandId == "totalMoneyDispensed")
                OnFinishDispenseSession((FinishDispenseResponse)args.Result);
            else if (args.CommandId == "moneyDispensedEvent")
                OnMoneyDispensed((MoneyMovementEvent)args.Result);
            else if (args.CommandId == "commandResponse")
            {
                var isValidResult = typeof(bool) == args.Result.GetType();
                if (isValidResult)
                {
                    if (args.ResponseId == 1)// startMoneyInsertion
                        GetStartMoneyInsertionCommandResponse((bool)args.Result);
                    else if (args.ResponseId == 12)// startDispensionSession
                        GetStartMoneyDispensionSessionCommandResponse((bool)args.Result);
                }
            }
        }

        private void GetStartMoneyInsertionCommandResponse(bool result)
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
                    Title = "Listo para recibir el cobro",
                    Message = "Para dispensar el cambio, presione 'Finalizar cobro'",
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

        private void GetStartMoneyDispensionSessionCommandResponse(bool result)
        {
            this.ZeusManager.IsInsertSessionOpen = result;
            if(result)
            {
                this.EventAggregator.GetEvent<StartListeningForResponseReceivedEvent>().Publish(new StartListeningForResponseReceivedEventArgs
                {
                    Response = "event",
                    EvenType = "moneyDispensedEvent"
                });
            }
        }

        private void OnFinishDispenseSession(FinishDispenseResponse result)
        {
            if(result.TotalMoneyDispensed >= this.ExchangeAmount)
            {
                HandleAsyncCall(async () =>
                {
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = new InfoBar()
                    {
                        IsOpen = true,
                        IsIconVisible = true,
                        IsClosable = false,
                        Severity = InfoBarSeverity.Success,
                        Title = "Cambio dispensado exitosamente!",
                        Message = "Presione Salir",
                        HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                        VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
                    };

                    dialog.PrimaryButtonText = "OK";

                    await dialog.ShowAsync();
                });
            }
        }

        private void OnMoneyDispensed(MoneyMovementEvent result)
        {
            var currentDispensedMoney = result;
            if (currentDispensedMoney != null)
            {
                m_dispensedExchangeAmount += currentDispensedMoney.Data[0].Value;
                if (m_dispensedExchangeAmount >= this.ExchangeAmount)
                {
                    HandleAsyncCall(async () =>
                    {
                        await this.ZeusManager.CloseDispenseSession();
                    });
                }

                //Log dispensed denominations

            }
        }


        private void OnUpdateInsertedMoney(MoneyMovementEvent movementEvent)
        {
            var currentInsertedMoney = movementEvent;
            if (currentInsertedMoney != null)
            {
                this.InsertedAmount += currentInsertedMoney.Data[0].Value;
                if (m_insertedAmount >= m_totalChargeAmount)
                {
                    HandleAsyncCall(async () =>
                    {
                        await this.ZeusManager.CloseMoneyInsertion();

                        if (this.ExchangeAmount > 0)
                            await this.ZeusManager.StartDispenseMoneySession(this.ExchangeAmount);
                        else
                        {
                            ContentDialog dialog = new ContentDialog();
                            dialog.Title = new InfoBar()
                            {
                                IsOpen = true,
                                IsIconVisible = true,
                                IsClosable = false,
                                Severity = InfoBarSeverity.Success,
                                Title = "Pago recibido con éxito!",
                                Message = "Presione Salir",
                                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
                            };

                            dialog.PrimaryButtonText = "OK";

                            await dialog.ShowAsync();
                        }
                    });
                }
            }
        }

        private bool m_isInserting = false;
        public bool IsInserting
        {
            get { return m_isInserting; }
            set { SetProperty(ref m_isInserting, value); }
        }

        private double m_totalChargeAmount;
        public double TotalChargeAmount
        {
            get { return m_totalChargeAmount; }
            set
            {
                SetProperty(ref m_totalChargeAmount, value);
                OnPropertyChanged("RemainingAmount");
                OnPropertyChanged("ExchangeAmount");
            }
        }

        private double m_insertedAmount;
        public double InsertedAmount
        {
            get { return m_insertedAmount; }
            set
            {
                SetProperty(ref m_insertedAmount, value);
                OnPropertyChanged("RemainingAmount");
                OnPropertyChanged("ExchangeAmount");
            }
        }
        public double RemainingAmount
        {
            get { return m_insertedAmount > m_totalChargeAmount ? 0 : m_totalChargeAmount - m_insertedAmount; }
        }

        public double ExchangeAmount
        {
            get { return m_insertedAmount > m_totalChargeAmount ? (m_insertedAmount - m_totalChargeAmount) : 0; }
        }

        private double m_dispensedExchangeAmount = 0;

        private IEventAggregator EventAggregator { get; set; }
    }
}
