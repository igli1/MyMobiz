using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Services
    {
        public Services()
        {
            Quotes = new HashSet<Quotes>();
            Referers = new HashSet<Referers>();
        }

        public string Id { get; set; }
        public string ServiceName { get; set; }
        public string ApiKey { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual ICollection<Quotes> Quotes { get; set; }
        public virtual ICollection<Referers> Referers { get; set; }
    }
}
