using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ULALA.UI.Core.MVVM;

namespace ULALA.Core.Contracts.Zeus.DTO
{

    public class WithdrawalCashModel : FundsInfoModel
    {
        public WithdrawalCashModel()
        {
        }

        private uint m_withdrawalQuantity;
        public uint WithdrawalQuantity
        {
            get { return m_withdrawalQuantity; }
            set 
            { 
                SetProperty(ref m_withdrawalQuantity, value);
                OnPropertyChanged("WithdrawalAmount"); // for WithdrawalAmount property binding update purposes
            }
        }

        private double m_withdrawalAmount;
        public double WithdrawalAmount
        {
            get { return m_withdrawalAmount = this.WithdrawalQuantity * this.Denomination; }
            set { SetProperty(ref m_withdrawalAmount, value); }
        }

        private bool m_isWithdrawn;
        public bool IsWithdrawn
        {
            get { return m_isWithdrawn; }
            set { SetProperty(ref m_isWithdrawn, value); }
        }
    }
}
