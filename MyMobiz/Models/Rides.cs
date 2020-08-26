using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Rides
    {
        public Rides()
        {
            Quotes = new HashSet<Quotes>();
            Rideslegs = new HashSet<Rideslegs>();
        }

        public string Id { get; set; }
        public string DriverRanking { get; set; }
        public string ClientRanking { get; set; }

        public virtual ICollection<Quotes> Quotes { get; set; }
        public virtual ICollection<Rideslegs> Rideslegs { get; set; }
    }
}
