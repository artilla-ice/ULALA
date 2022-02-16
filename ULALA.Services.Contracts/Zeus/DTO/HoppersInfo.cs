using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO
{
    public class HoppersInfo
    {
        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("coinsInfo")]
        public CoinInfo[] CoinsInfo { get; set; }

        public HoppersInfo()
        {
            CoinsInfo = new CoinInfo[2]
            {
                new CoinInfo(),
                new CoinInfo(),
            };
        }
    }
}
