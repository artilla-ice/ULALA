using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ULALA.Services.Contracts.Zeus.DTO.Status
{
    public class BillRecyclerStatus
    {
        [JsonProperty("canAcceptBills")]
        public bool CanAcceptBills { get; set; }

        [JsonProperty("canDispenseBills")]
        public bool CanDispenseBills { get; set; }
    }
}
