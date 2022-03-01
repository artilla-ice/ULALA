using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Models;
using ULALA.UI.Core.Contracts.Navigation;
using ULALA.UI.Core.MVVM;
using ULALA.Views;
using Xamarin.Forms;

namespace ULALA.ViewModels
{
    public class CashFundsViewModel : ViewModelBase
    {
        public Command CashOutCommand { get; set; }
        public Command AutomaticFundCommand { get; set; }
        public CashFundsViewModel()
        {
            this.CashOutCommand = new Command(OnCashOut);
            this.AutomaticFundCommand = new Command(OnAutomaticFund);
        }

        protected override void OnActivated()
        {
            this.PageIcon = "../Assets/Icons/Estatus.png"; //TODO: agregar path de iconos a archivo de recursos

            OnLoadRecyclerAmounts();
        }

        private void OnCashOut()
        {
            this.NavigationManager.NavigateTo(ViewNames.CashOut);
        }

        private void OnAutomaticFund()
        {
            var i = 0;
            var x = i++;
        }

        private void OnLoadRecyclerAmounts()
        {
            var amounts = new List<RecyclerToCashierFundsModel>()
            {
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 200,
                    RecyclerQuantity = 10
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 500,
                    RecyclerQuantity = 5
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 20,
                    RecyclerQuantity = 25
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Coins,
                    Denomination = 10,
                    RecyclerQuantity = 78
                },
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Coins,
                    Denomination = 5,
                    RecyclerQuantity = 80
                }
                ,
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Coins,
                    Denomination = .50,
                    RecyclerQuantity = 80
                }
            };

            amounts = amounts.OrderByDescending(a => a.Denomination).ToList();
            this.RecyclerAmounts = new ObservableCollection<RecyclerToCashierFundsModel>(amounts);
        }

        private ObservableCollection<RecyclerToCashierFundsModel> m_recyclerAmounts;
        public ObservableCollection<RecyclerToCashierFundsModel> RecyclerAmounts
        {
            get { return m_recyclerAmounts; }
            set { SetProperty(ref m_recyclerAmounts, value); }
        }

        private string m_virtualKeyboardValue;
        public string VirtualKeyboardValue
        {
            get { return m_virtualKeyboardValue; }
            set { SetProperty(ref m_virtualKeyboardValue, value); }
        }
    }
}
