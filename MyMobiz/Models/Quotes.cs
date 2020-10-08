using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Quotes
    {
        public Quotes()
        {
            Orders = new HashSet<Orders>();
            Rides = new HashSet<Rides>();
        }

        public string Id { get; set; }
        public string ServiceId { get; set; }
        public int RefererId { get; set; }
        public int? VerNum { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }
        public decimal? Price { get; set; }

        public virtual Webreferers Referer { get; set; }
        public virtual Services Service { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Rides> Rides { get; set; }
    }
}
