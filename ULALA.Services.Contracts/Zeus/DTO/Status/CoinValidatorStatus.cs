using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Zeus.DTO.Status
{
    public class CoinValidatorStatus
    {
        [JsonProperty("canAcceptCoins")]
        public bool CanAcceptCoins { get; set; }

        [JsonProperty("canDispenseCoins")]
        public bool CanDispenseCoins { get; set; }
    }
}
