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

    public class RecyclerToCashierFundsModel : FundsInfoModel
    {
        public RecyclerToCashierFundsModel()
        {
        }

        private int m_cashierQuantity;
        public int CashierQuantity
        {
            get { return m_cashierQuantity; }
            set { SetProperty(ref m_cashierQuantity, value); }
        }

        public double CashierAmount
        {
            get { return this.CashierQuantity * this.Denomination; }
        }

        private int m_missingQuantity;
        public int MissingQuantity
        {
            get { return m_missingQuantity; }
            set { SetProperty(ref m_missingQuantity, value); }
        }

        private int m_surplusQuantity;
        public int SurplusQuantity
        {
            get { return m_surplusQuantity; }
            set { SetProperty(ref m_surplusQuantity, value); }
        }
    }
}
