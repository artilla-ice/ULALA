using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Zeus.DTO.CashRetrieval
{
    public class RetrievedDenominationInfo
    {
        [JsonProperty("value")]
        public double Value { get; set; }
        [JsonProperty("count")]
        public uint Count { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
