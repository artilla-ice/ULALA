using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Models;
using ULALA.UI.Core.MVVM;

namespace ULALA.ViewModels
{
    public class AddExchangeViewModel : ViewModelBase
    {
        public AddExchangeViewModel()
        {

            OnLoadRecyclerAmounts();
        }

        private void OnLoadRecyclerAmounts()
        {
            var amounts = new List<RecyclerToCashierFundsModel>()
            {
                new RecyclerToCashierFundsModel
                {
                    CashType = CashType.Bills,
                    Denomination = 1000,
                    RecyclerQuantity = 3
                },
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
    }
}
