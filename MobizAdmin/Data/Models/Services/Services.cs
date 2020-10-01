using System;
using System.Collections.Generic;

namespace MobizAdmin.Data
{
    public partial class Services
    {
        public Services()
        {
            Referers = new HashSet<Referers>();
            Servicerates = new HashSet<Servicerates>();
        }

        public string Id { get; set; }
        public string ServiceName { get; set; }
        public string ApiKey { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual ICollection<Referers> Referers { get; set; }
        public virtual ICollection<Servicerates> Servicerates { get; set; }
    }
}
