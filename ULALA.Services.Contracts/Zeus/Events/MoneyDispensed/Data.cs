using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Events.MoneyDispensed
{
    public class Data
    {
        [JsonProperty("count")]
        public uint Count { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }
}
