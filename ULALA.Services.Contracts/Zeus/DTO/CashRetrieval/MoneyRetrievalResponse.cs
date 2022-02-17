using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO.CashRetrieval
{
    public class MoneyRetrievalResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        public MoneyRetrievalResponse()
        {
            Data = new Data();
        }
    }
}
