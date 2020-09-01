using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Legs
    {
        public Legs()
        {
            Rideslegs = new HashSet<Rideslegs>();
        }

        public int Id { get; set; }
        public int FromPlaceId { get; set; }
        public int ToPlaceId { get; set; }
        public int? Kms { get; set; }
        public int? MinutesDrive { get; set; }
        public int? MinutesWithTraffic { get; set; }
        public double? Fare { get; set; }

        public virtual Placess FromPlace { get; set; }
        public virtual Placess ToPlace { get; set; }
        public virtual ICollection<Rideslegs> Rideslegs { get; set; }
    }
}
