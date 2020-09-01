using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Quotes
    {
        public Quotes()
        {
            Orders = new HashSet<Orders>();
        }

        public string Id { get; set; }
        public string ServiceId { get; set; }
        public int RefererId { get; set; }
        public string RideId { get; set; }
        public int? VerNum { get; set; }
        public double? Price { get; set; }
        
        public virtual Referers Referer { get; set; }
        public virtual Rides Ride { get; set; }
        public virtual Services Service { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
