using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ULALA.Services.Contracts.Zeus.DTO.Status
{
    public class Warning
    {
        [JsonProperty("code")]
        public uint Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
