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

namespace ULALA.ViewModels
{
    public class WithdrawCashViewModel : ViewModelBase
    {
        [Dependency]
        public IZeusManager ZeusManager { get; set; }
        public WithdrawCashViewModel()
        {

        }

        protected override void OnActivated()
        {
            this.PageIcon = "../Assets/Icons/RetirarEfectivo.png";
            OnLoadRecyclerAmounts();

            
        }
        private void OnLoadRecyclerAmounts()
        {
            var recyclerValues = this.ZeusManager.GetRecyclerValues();
            recyclerValues = recyclerValues.OrderByDescending(a => a.Denomination).ToList();
            this.RecyclerAmounts = new ObservableCollection<WithdrawalCashModel>(recyclerValues);
        }

        private ObservableCollection<WithdrawalCashModel> m_recyclerAmounts;
        public ObservableCollection<WithdrawalCashModel> RecyclerAmounts
        {
            get { return m_recyclerAmounts; }
            set { SetProperty(ref m_recyclerAmounts, value); }
        }

    }
}
