using System;
using System.Collections.Generic;

namespace MyMobiz.New
{
    public partial class Legs
    {
        public Legs()
        {
            Ridelegs = new HashSet<Ridelegs>();
        }

        public int Id { get; set; }
        public int FromPlaceId { get; set; }
        public int ToPlaceId { get; set; }
        public int? Kms { get; set; }
        public int? MinutesDrive { get; set; }
        public int? MinutesWithTraffic { get; set; }
        public double? Fare { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Places FromPlace { get; set; }
        public virtual Places ToPlace { get; set; }
        public virtual ICollection<Ridelegs> Ridelegs { get; set; }
    }
}
