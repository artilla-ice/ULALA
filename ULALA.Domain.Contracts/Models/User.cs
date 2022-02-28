using System;
using System.Collections.Generic;
using System.Text;

namespace ULALA.Domain.Contracts.Models
{
    public class User
    {
        public User()
        {
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Device Device { get; set; }
        public Role Role { get; set; }
    }
}
