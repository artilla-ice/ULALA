using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ULALA.Domain.Contracts.Models
{
    public class Device
    {
        public Device()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
    }
}
