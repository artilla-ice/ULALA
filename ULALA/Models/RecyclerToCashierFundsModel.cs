using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ULALA.UI.Core.MVVM;
using Xamarin.Forms;

namespace ULALA.Models
{
    public enum CashType
    {
        Bills = 0x0001,
        Coins = 0x0002
    }

    public class RecyclerToCashierFundsModel : ModelBase
    {
        public RecyclerToCashierFundsModel()
        {
        }

        public string Title { get => GetTitle(); }

        private CashType m_cashType;
        public CashType CashType
        {
            get { return m_cashType; }
            set { SetProperty(ref m_cashType, value); }
        }

        private double m_denomination;
        public double Denomination
        {
            get { return m_denomination; }
            set { SetProperty(ref m_denomination, value); }
        }

        public string DenominationIcon
        {
            get => GetDenominationIcon(); 
        }

        public int DenominationIconSize
        {
            get
            {
                return (this.CashType == CashType.Bills) ? 90 : 40;
            }
        }

        private int m_recyclerQuantity;
        public int RecyclerQuantity
        {
            get { return m_recyclerQuantity; }
            set { SetProperty(ref m_recyclerQuantity, value); }
        }

        public double RecyclerAmount
        {
            get { return this.RecyclerQuantity * this.Denomination; }
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

        private string GetDenominationIcon()
        {
            var iconName = string.Empty;

            iconName = (this.CashType == CashType.Bills) ? "Bill" : "Mon";

            //Check if denomination is 50 cents
            iconName += (this.Denomination == .50) ? "050" : this.Denomination.ToString();

            var iconPath = string.Format("../Assets/Icons/{0}.png", iconName);
            return iconPath;
        }

        private string GetTitle()
        {
            var title = (this.Denomination == .50) ? string.Format("¢{0}", (this.Denomination*100)) 
                                                        : string.Format("${0}", this.Denomination);

            return title;
        }
    }
}
