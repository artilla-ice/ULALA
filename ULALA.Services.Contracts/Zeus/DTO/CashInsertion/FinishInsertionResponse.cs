using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO.CashInsertion
{
    public class FinishInsertionResponse
    {
        [JsonProperty("totalMoneyInserted")]
        public double TotalMoneyInserted { get; set; }

        [JsonProperty("insertedDenominationsInfo")]
        public InsertedDenominationInfo[] InsertedDenominationsInfo { get; set; }
    }
}
