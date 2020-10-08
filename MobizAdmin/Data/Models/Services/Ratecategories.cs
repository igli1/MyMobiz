using System;
using System.Collections.Generic;

namespace MobizAdmin.Data
{
    public partial class Ratecategories
    {
        public Ratecategories()
        {
            Ratedetails = new HashSet<Ratedetails>();
        }

        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string Lexo { get; set; }
        public string RateGrouping { get; set; }
        public string CategoryConditions { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Rategroupings RateGroupingNavigation { get; set; }
        public virtual Services Service { get; set; }
        public virtual ICollection<Ratedetails> Ratedetails { get; set; }
    }
}
