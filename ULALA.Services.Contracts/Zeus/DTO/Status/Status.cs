using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Zeus.DTO.Status
{
    public class Status
    {
        [JsonProperty("billRecyclerStatus")]
        public BillRecyclerStatus BillRecyclerStatus { get; set; }

        [JsonProperty("coinValidatorStatus")]
        public CoinValidatorStatus CoinValidatorStatus { get; set; }

        [JsonProperty("errors")]
        public Error[] Errors { get; set; }

        [JsonProperty("warnings")]
        public Warning[] Warnings { get; set; }

        public Status()
        {
            BillRecyclerStatus = new BillRecyclerStatus();
            CoinValidatorStatus = new CoinValidatorStatus();
        }
    }
}
