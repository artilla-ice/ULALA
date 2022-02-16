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
    public enum CashType
    {
        Bills = 0x0001,
        Coins = 0x0002
    }

    public class FundsInfoModel : ModelBase
    {
        public FundsInfoModel()
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

        private uint m_recyclerQuantity;
        public uint RecyclerQuantity
        {
            get { return m_recyclerQuantity; }
            set { SetProperty(ref m_recyclerQuantity, value); }
        }

        public double RecyclerAmount
        {
            get { return this.RecyclerQuantity * this.Denomination; }
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
