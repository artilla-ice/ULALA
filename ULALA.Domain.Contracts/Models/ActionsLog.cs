using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ULALA.Domain.Contracts.Models
{
    public class ActionsLog
    {
        public ActionsLog()
        {
        }

        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public Device Device { get; set; }
    }
}
