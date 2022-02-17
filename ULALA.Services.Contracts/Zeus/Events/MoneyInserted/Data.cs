using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Events.MoneyInserted
{
    public class Data
    {
        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("route")]
        public string Route { get; set; }
    }
}
