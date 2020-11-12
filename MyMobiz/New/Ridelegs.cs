using System;
using System.Collections.Generic;

namespace MyMobiz.New
{
    public partial class Ridelegs
    {
        public int Id { get; set; }
        public int LegId { get; set; }
        public string RideId { get; set; }
        public int? Seqnr { get; set; }
        public DateTime? DateTimeArrival { get; set; }
        public DateTime? DateTimePickup { get; set; }
        public DateTime? DateTimeArrivalTh { get; set; }
        public DateTime? DateTimePickupTh { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Legs Leg { get; set; }
        public virtual Rides Ride { get; set; }
    }
}
