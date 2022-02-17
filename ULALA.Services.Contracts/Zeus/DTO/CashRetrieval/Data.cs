using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO.CashRetrieval
{
    public class Data
    {
        [JsonProperty("totalMoneyRetrieved")]
        public double TotalMoneyRetrieved { get; set; }

        [JsonProperty("retrivedDenominationsInfo")]
        public RetrievedDenominationInfo[] RetrievedDenominationsInfo { get; set; }
    }
}
