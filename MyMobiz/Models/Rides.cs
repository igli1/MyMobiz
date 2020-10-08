using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Rides
    {
        public Rides()
        {
            Ridelegs = new HashSet<Ridelegs>();
        }

        public string Id { get; set; }
        public string DriverRanking { get; set; }
        public string ClientRanking { get; set; }
        public string QuoteId { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Quotes Quote { get; set; }
        public virtual ICollection<Ridelegs> Ridelegs { get; set; }
    }
}
