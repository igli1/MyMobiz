using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Rideslegs
    {
        public int Id { get; set; }
        public int LegId { get; set; }
        public string RideId { get; set; }
        public int? Seqnr { get; set; }
        public DateTime? DateTimeArrival { get; set; }
        public DateTime? DateTimePickup { get; set; }
        public DateTime? DateTimeArrivalTh { get; set; }
        public DateTime? DateTimePickupTh { get; set; }

        public virtual Legs Leg { get; set; }
        public virtual Rides Ride { get; set; }
    }
}
