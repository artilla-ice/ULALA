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

    public class WithdrawalStackerCashModel : FundsInfoModel
    {
        public WithdrawalStackerCashModel()
        {
        }

        private uint m_stackerQuantity;
        public uint StackerQuantity
        {
            get { return m_stackerQuantity; }
            set 
            { 
                SetProperty(ref m_stackerQuantity, value);
                OnPropertyChanged("StackerAmount"); // for StackerAmount property binding update purposes
            }
        }

        private double m_stackerAmount;
        public double StackerAmount
        {
            get { return m_stackerAmount = this.StackerQuantity * this.Denomination; }
            set { SetProperty(ref m_stackerAmount, value); }
        }
    }
}
