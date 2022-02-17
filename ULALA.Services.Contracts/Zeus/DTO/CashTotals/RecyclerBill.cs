using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO.CashTotals
{
    public class RecyclerBill
    {
        [JsonProperty("boxId")]
        public uint BoxId { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("count")]
        public uint Count { get; set; }
    }
}
