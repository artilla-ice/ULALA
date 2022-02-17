using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Zeus.DTO.Status
{
    public class StatusResponse
    {
        [JsonProperty("status")]
        public Status Status { get; set; }

        public StatusResponse()
        {
            Status = new Status();
        }
    }
}
