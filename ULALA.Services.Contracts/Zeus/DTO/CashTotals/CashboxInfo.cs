using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO.CashTotals
{
    public class CashboxInfo
    {
        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("coinsInfo")]
        public CoinInfo[] CoinsInfo { get; set; }

        public CashboxInfo()
        {
            CoinsInfo = new CoinInfo[5]
            {
                new CoinInfo(),
                new CoinInfo(),
                new CoinInfo(),
                new CoinInfo(),
                new CoinInfo(),
            };
        }
    }
}
