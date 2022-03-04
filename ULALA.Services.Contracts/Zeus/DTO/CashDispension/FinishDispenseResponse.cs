using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Zeus.DTO.CashDispension
{
    public class FinishDispenseResponse
    {
        [JsonProperty("totalMoneyDispensed")]
        public double TotalMoneyDispensed { get; set; }

        [JsonProperty("dispensedDenominationsInfo")]
        public DispensedDenominationInfo[] DispensedDenominationsInfo { get; set; }
    }
}
