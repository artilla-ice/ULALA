using System;
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
using Unity;
using Xamarin.Forms;

namespace ULALA.ViewModels
{
    public class WithdrawCashViewModel : ViewModelBase
    {
        [Unity.Dependency]
        public IZeusManager ZeusManager { get; set; }

        public Command<WithdrawalCashModel> SetTotalQuantityWithdrawalCommand { get; }
        public Command<WithdrawalCashModel> WithdrawalSelectedCurrencyCommand { get; }
        public Command<WithdrawalCashModel> WithdrawalAllCurrenciesCommand { get; }


        public WithdrawCashViewModel()
        {
            this.SetTotalQuantityWithdrawalCommand = new Command<WithdrawalCashModel>((param) => OnSetTotalQuantities(param));
        }

        protected override void OnActivated()
        {
            this.PageIcon = "../Assets/Icons/RetirarEfectivo.png";
            OnLoadRecyclerAmounts();

            
        }

        private void OnSetTotalQuantities(WithdrawalCashModel withdrawalCashInfo)
        {
            var currentRecyclerInfo = this.RecyclerAmounts.Where(r => r.Denomination == withdrawalCashInfo.Denomination)
                                                            .FirstOrDefault();

            currentRecyclerInfo.WithdrawalQuantity = currentRecyclerInfo.RecyclerQuantity;
        }

        private void OnLoadRecyclerAmounts()
        {
            var recyclerValues = this.ZeusManager.GetRecyclerValues();
            if(recyclerValues != null)
            {
                recyclerValues = recyclerValues.OrderByDescending(a => a.Denomination).ToList();
                this.RecyclerAmounts = new ObservableCollection<WithdrawalCashModel>(recyclerValues);
            }
        }

        private ObservableCollection<WithdrawalCashModel> m_recyclerAmounts;
        public ObservableCollection<WithdrawalCashModel> RecyclerAmounts
        {
            get { return m_recyclerAmounts; }
            set { SetProperty(ref m_recyclerAmounts, value); }
        }

    }
}
