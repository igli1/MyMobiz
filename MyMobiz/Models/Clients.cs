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
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
