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

        }

        private void SubscribeToEvents()
        {
            
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
                    Title = "Listo para recibir el cobro",
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

        private IEventAggregator EventAggregator { get; set; }
    }
}
