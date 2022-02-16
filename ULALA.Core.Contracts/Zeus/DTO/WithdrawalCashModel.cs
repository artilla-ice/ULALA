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

        private int m_withdrawalQuantity;
        public int WithdrawalQuantity
        {
            get { return m_withdrawalQuantity; }
            set { SetProperty(ref m_withdrawalQuantity, value); }
        }

        public double WithdrawalAmount
        {
            get { return this.WithdrawalQuantity * this.Denomination; }
        }

        private bool m_isWithdrawn;
        public bool IsWithdrawn
        {
            get { return m_isWithdrawn; }
            set { SetProperty(ref m_isWithdrawn, value); }
        }
    }
}
