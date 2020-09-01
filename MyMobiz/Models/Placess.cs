using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Placess
    {
        public Placess()
        {
            LegsFromPlace = new HashSet<Legs>();
            LegsToPlace = new HashSet<Legs>();
        }

        public int Id { get; set; }
        public string Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }

        public virtual ICollection<Legs> LegsFromPlace { get; set; }
        public virtual ICollection<Legs> LegsToPlace { get; set; }
    }
}
