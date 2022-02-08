using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ULALA.UI.Core.MVVM;
using Xamarin.Forms;

namespace ULALA.Models
{
    public class CashOutModel : ModelBase
    {
        public CashOutModel()
        {
        }

        private Guid m_id;
        [Display(AutoGenerateField = false)]
        public Guid Id
        {
            get { return m_id; }
            set { SetProperty(ref m_id, value); }
        }

        private DateTime m_startedDate;
        [Display(Name = "Fecha")]
        public DateTime StartedDate
        {
            get { return m_startedDate; }
            set { SetProperty(ref m_startedDate, value); }
        }

        private DateTime m_endDate;
        [Display(AutoGenerateField = false)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime EndDate
        {
            get { return m_endDate; }
            set { SetProperty(ref m_endDate, value); }
        }

        private double m_initialAmount;
        [Display(Name = "Inicial")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public double InitialAmount
        {
            get { return m_initialAmount; }
            set { SetProperty(ref m_initialAmount, value); }
        }

        private double m_incomingAmount;
        [Display(Name = "Entradas")]
        public double IncomingAmount
        {
            get { return m_incomingAmount; }
            set { SetProperty(ref m_incomingAmount, value); }
        }

        private double m_withdrawalAmount;
        [Display(Name = "Retiros")]
        public double WithdrawalAmount
        {
            get { return m_withdrawalAmount; }
            set { SetProperty(ref m_withdrawalAmount, value); }
        }

        private double m_salesAmount;
        [Display(Name = "Ventas")]
        public double SalesAmount
        {
            get { return m_salesAmount; }
            set { SetProperty(ref m_salesAmount, value); }
        }

        private double m_finalAmount;
        [Display(Name = "Final")]
        public double FinalAmount
        {
            get { return m_finalAmount; }
            set { SetProperty(ref m_finalAmount, value); }
        }

        private double m_balance;
        [Display(Name = "Saldo")]
        public double Balance
        {
            get { return m_balance; }
            set { SetProperty(ref m_balance, value); }
        }
    }
}
