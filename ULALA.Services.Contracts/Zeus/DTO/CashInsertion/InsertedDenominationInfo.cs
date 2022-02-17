using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Zeus.DTO.CashInsertion
{
    public class InsertedDenominationInfo
    {
        [JsonProperty("value")]
        public double Value { get; set; }
        
        [JsonProperty("count")]
        public uint Count { get; set; }
        
        [JsonProperty("route")]
        public string Route { get; set; }
    }
}
