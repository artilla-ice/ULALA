using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO.CashTotals
{
    public class RecyclersInfo
    {
        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("recyclerBills")]
        public RecyclerBill[] RecyclerBills { get; set; }

        public RecyclersInfo()
        {
            RecyclerBills = new RecyclerBill[2]
            {
                new RecyclerBill(),
                new RecyclerBill(),
            };
        }
    }
}
