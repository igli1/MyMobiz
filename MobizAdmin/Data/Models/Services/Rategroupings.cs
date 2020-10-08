using System;
using System.Collections.Generic;

namespace MobizAdmin.Data
{
    public partial class Rategroupings
    {
        public Rategroupings()
        {
            Ratecategories = new HashSet<Ratecategories>();
        }

        public string RateGrouping { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual ICollection<Ratecategories> Ratecategories { get; set; }
    }
}
