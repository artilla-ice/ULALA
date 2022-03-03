using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ULALA.Core.Contracts.Events;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Infrastructure.PubSub;
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

            SubscribeToEvents();
        }

        protected override void OnActivated()
        {
            this.PageIcon = "../Assets/Icons/Cobrar.png"; //TODO: agregar path de iconos a archivo de recursos

            OnStartMoneyInsertion();
        }


        private void OnStartMoneyInsertion()
        {
            var isReadyForInsertion = this.ZeusManager.StartMoneyInsertion();
            this.IsInserting = isReadyForInsertion;

            HandleAsyncCall(async () =>
            {
                if(isReadyForInsertion)
                {
                    ContentDialog dialog = new ContentDialog();
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

                    dialog.PrimaryButtonText = "OK";

                    await dialog.ShowAsync();
                }
                else
                {
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = new InfoBar()
                    {
                        IsOpen = true,
                        IsIconVisible = true,
                        IsClosable = false,
                        Severity = InfoBarSeverity.Error,
                        Title = "Ha ocurrido un error. Comprueba la conexión con el Stacker",
                        HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch,
                        VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Stretch
                    };

                    dialog.PrimaryButtonText = "OK";

                    await dialog.ShowAsync();
                }
            });
        }

        private void OnFinalizeMoneyInsertion()
        {
            HandleAsyncCall(async () =>
            {
                await this.ZeusManager.CloseMoneyInsertion();

                this.IsInserting = false;

                this.ZeusManager.StartDispenseMoneySession(this.ExchangeAmount);
                //await this.ZeusManager.CloseDispenseSession(); //TODO: verificar si hay que cerrar la dispensacion desde el viewmodel o desde el emulador

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
            this.EventAggregator.GetEvent<NewMoneyInsertEvent>()
                .Subscribe(async (args) =>
                {
                    var currentInsertedMoney = args.Response;
                    if (currentInsertedMoney != null)
                    {
                        this.InsertedAmount += currentInsertedMoney.Data[0].Value;
                        if(m_insertedAmount >= m_totalChargeAmount)
                        {
                            await this.ZeusManager.CloseMoneyInsertion();

                            if(this.ExchangeAmount > 0)
                                this.ZeusManager.StartDispenseMoneySession(this.ExchangeAmount);
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
                        }
                    }
                }, ThreadOption.UIThread);

            this.EventAggregator.GetEvent<NewMoneyDispensedEvent>()
                .Subscribe(async (args) =>
                {
                    var currentDispensedMoney = args.Response;
                    if (currentDispensedMoney != null)
                    {
                        m_dispensedExchangeAmount += currentDispensedMoney.Data[0].Value;
                        if(m_dispensedExchangeAmount >= this.ExchangeAmount)
                        {
                            await this.ZeusManager.CloseDispenseSession();

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
                        }    

                        //Log dispensed denominations
                    }
                }, ThreadOption.UIThread);
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
