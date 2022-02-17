using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ULALA.Core.Contracts.Zeus;
using ULALA.Core.Contracts.Zeus.DTO;
using ULALA.Models;
using ULALA.UI.Core.MVVM;
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
            this.StartInsertionCommand = new Command(OnStartMoneyInsertion);
        }

        protected override void OnActivated()
        {
            OnLoadAllDenominationsInfo();
        }

        private void OnStartMoneyInsertion()
        {
            var isReadyForInsertion = this.ZeusManager.StartMoneyInsertion();
            this.IsInserting = isReadyForInsertion;

            HandleAsyncCall(async () =>
            {
                var currentInsertedMoney = await this.ZeusManager.GetEventResponse();
                if(currentInsertedMoney != null)
                {
                    var itemOnCollection = this.RecyclerAmounts.Where(r => currentInsertedMoney.Type == "moneyInsertedEvent"
                                                    && currentInsertedMoney.Data[0] != null
                                                    && currentInsertedMoney.Data[0].Value == r.Denomination).FirstOrDefault();

                    var currentQuantity = itemOnCollection.RecyclerQuantity;
                    itemOnCollection.RecyclerQuantity = ++currentQuantity;
                }
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
    }
}
