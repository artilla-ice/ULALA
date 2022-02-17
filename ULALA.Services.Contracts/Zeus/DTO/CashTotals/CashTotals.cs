using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO.CashTotals
{
    public class CashTotals
    {
        [JsonProperty("coinsTotal")]
        public double CoinsTotal { get; set; }

        [JsonProperty("cashBoxInfo")]
        public CashboxInfo CashBoxInfo { get; set; }

        [JsonProperty("hoppersInfo")]
        public HoppersInfo HoppersInfo { get; set; }

        [JsonProperty("billsTotal")]
        public double BillsTotal { get; set; }

        [JsonProperty("stackerInfo")]
        public StackerInfo StackerInfo { get; set; }

        [JsonProperty("recyclersInfo")]
        public RecyclersInfo RecyclersInfo { get; set; }

        public CashTotals()
        {
            CashBoxInfo = new CashboxInfo();
            HoppersInfo = new HoppersInfo();
            StackerInfo = new StackerInfo();
            RecyclersInfo = new RecyclersInfo();
        }
    }
}
