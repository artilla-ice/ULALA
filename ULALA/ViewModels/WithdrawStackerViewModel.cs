﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Models;
using ULALA.UI.Core.MVVM;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms;

namespace ULALA.ViewModels
{
    public class WithdrawStackerViewModel : ViewModelBase
    {
        [Unity.Dependency]
        public IZeusManager ZeusManager { get; set; }

        public Command RetrieveStackerCashCommand { get; }

        public WithdrawStackerViewModel()
        {
            this.RetrieveStackerCashCommand = new Command(OnRetrieveCash);
        }

        protected override void OnActivated()
        {
            this.PageIcon = "../Assets/Icons/Cobrar.png";

            OnLoadStackerAmounts();
        }

        private void OnRetrieveCash()
        {
            HandleAsyncCall(async () =>
            {
                var result = await ZeusManager.RetriveStackerCash();
                if(result.Data.TotalMoneyRetrieved == this.StackerTotalAmount)
                {
                    ContentDialog dialog = new ContentDialog();
                    dialog.Title = "Retiro exitoso";
                    dialog.PrimaryButtonText = "OK";
                    dialog.Content = "El stacker ha sido vaciado";

                    var rsponse = await dialog.ShowAsync();

                    OnLoadStackerAmounts();
                }
            });
        }

        private void OnLoadStackerAmounts()
        {
            var stackerValues = this.ZeusManager.GetStackerValues();
            if (stackerValues != null)
            {
                stackerValues = stackerValues.OrderByDescending(a => a.Denomination).ToList();
                this.StackerAmounts = new ObservableCollection<WithdrawalStackerCashModel>(stackerValues);

                CalculateTotalAmount();
            }
        }

        private void CalculateTotalAmount()
        {
            this.StackerTotalAmount = m_stackerAmounts.Sum(a => a.StackerAmount);
        }

        private ObservableCollection<WithdrawalStackerCashModel> m_stackerAmounts;
        public ObservableCollection<WithdrawalStackerCashModel> StackerAmounts
        {
            get { return m_stackerAmounts; }
            set { SetProperty(ref m_stackerAmounts, value); }
        }

        private double m_stackerTotalAmount;
        public double StackerTotalAmount
        {
            get { return m_stackerTotalAmount; }
            set { SetProperty(ref m_stackerTotalAmount, value); }
        }
    }
}
