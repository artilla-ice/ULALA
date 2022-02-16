using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO
{
    public class CashTotalsResponse
    {
        [JsonProperty("cashTotals")]
        public CashTotals CashTotals{ get; set; }

        public CashTotalsResponse()
        {
            CashTotals = new CashTotals();
        }
    }
}
