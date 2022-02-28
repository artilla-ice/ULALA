using System;
using System.Collections.Generic;
using System.Text;

namespace ULALA.Domain.Contracts.Models
{
    public class Role
    {
        public Role()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Device Device { get; set; }
    }
}
