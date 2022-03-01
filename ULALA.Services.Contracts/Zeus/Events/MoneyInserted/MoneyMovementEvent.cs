using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Events.MoneyInserted
{
    public class MoneyMovementEvent
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public Data[] Data { get; set; }

        public MoneyMovementEvent()
        {
            
        }
    }
}
