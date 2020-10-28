using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Ratedetails
    {
        public Ratedetails()
        {
            Ratetargets = new HashSet<Ratetargets>();
        }

        public int Id { get; set; }
        public int Vernum { get; set; }
        public int CategoryId { get; set; }
        public string DetailConditions { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Ratecategories Category { get; set; }
        public virtual Servicerates VernumNavigation { get; set; }
        public virtual ICollection<Ratetargets> Ratetargets { get; set; }
    }
}
