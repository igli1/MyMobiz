using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Possibleratetargets
    {
        public Possibleratetargets()
        {
            Ratetargets = new HashSet<Ratetargets>();
        }

        public string PossibleTarget { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual ICollection<Ratetargets> Ratetargets { get; set; }
    }
}
