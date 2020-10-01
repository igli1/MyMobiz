using System;
using System.Collections.Generic;

namespace MobizAdmin.Data
{
    public partial class Ratecategories
    {
        public Ratecategories()
        {
            Ratesdetails = new HashSet<Ratesdetails>();
        }

        public string CategoryId { get; set; }
        public string RateGrouping { get; set; }
        public string Naming { get; set; }
        public string CategoryConditions { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Rategroupings RateGroupingNavigation { get; set; }
        public virtual ICollection<Ratesdetails> Ratesdetails { get; set; }
    }
}
