using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ULALA.Services.Contracts.Zeus.DTO
{
    public class StackerInfo
    {
        [JsonProperty("route")]
        public string Route { get; set; }

        [JsonProperty("billsInfo")]
        public BillInfo[] BillsInfo { get; set; }

        public StackerInfo()
        {
            BillsInfo = new BillInfo[6]
            {
                new BillInfo(),
                new BillInfo(),
                new BillInfo(),
                new BillInfo(),
                new BillInfo(),
                new BillInfo(),
            };
        }
    }
}
