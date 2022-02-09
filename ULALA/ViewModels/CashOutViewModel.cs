using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ULALA.Models;
using ULALA.UI.Core.MVVM;

namespace ULALA.ViewModels
{
    public class CashOutViewModel : ViewModelBase
    {
        public CashOutViewModel()
        {

        }

        protected override void OnActivated()
        {
            this.PageIcon = "../Assets/Icons/CashRegister.png"; //TODO: agregar path de iconos a archivo de recursos

            OnLoadCashOutMovements();
        }

        private void OnLoadCashOutMovements()
        {
            var movements = new List<CashOutModel>()
            {
                new CashOutModel
                {
                    StartedDate = DateTime.Now,
                    InitialAmount = 48564.50,
                    IncomingAmount = 23456.50,
                    WithdrawalAmount = 47545,
                    SalesAmount = 18343.5,
                    FinalAmount = 23544,
                    Balance = -25999,
                    EndDate = DateTime.Now.AddDays(3)
                },
                new CashOutModel
                {
                    StartedDate = DateTime.Now.AddDays(1),
                    InitialAmount = 48564.50,
                    IncomingAmount = 23456.50,
                    WithdrawalAmount = 47545,
                    SalesAmount = 18343.5,
                    FinalAmount = 23544,
                    Balance = -25999,
                    EndDate = DateTime.Now.AddDays(3)
                },
                new CashOutModel
                {
                    StartedDate = DateTime.Now.AddDays(2),
                    InitialAmount = 48564.50,
                    IncomingAmount = 23456.50,
                    WithdrawalAmount = 47545,
                    SalesAmount = 18343.5,
                    FinalAmount = 23544,
                    Balance = -25999,
                    EndDate = DateTime.Now.AddDays(3)
                },
                new CashOutModel
                {
                    StartedDate = DateTime.Now.AddDays(3),
                    InitialAmount = 48564.50,
                    IncomingAmount = 23456.50,
                    WithdrawalAmount = 47545,
                    SalesAmount = 18343.5,
                    FinalAmount = 23544,
                    Balance = -25999,
                    EndDate = DateTime.Now.AddDays(3)
                },
            };

            movements = movements.OrderByDescending(a => a.StartedDate).ToList();
            this.CashOutMovements = new ObservableCollection<CashOutModel>(movements);
        }

        private CashOutModel m_cashOutSelectedMovement = new CashOutModel();
        public CashOutModel CashOutSelectedMovement
        {
            get { return m_cashOutSelectedMovement; }
            set { SetProperty(ref m_cashOutSelectedMovement, value); }
        }

        private ObservableCollection<CashOutModel> m_cashOutMovements;
        public ObservableCollection<CashOutModel> CashOutMovements
        {
            get { return m_cashOutMovements; }
            set { SetProperty(ref m_cashOutMovements, value); }
        }
    }
}
