using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Clients
    {
        public Clients()
        {
            Orders = new HashSet<Orders>();
        }

        public string Id { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
