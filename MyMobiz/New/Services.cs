using System;
using System.Collections.Generic;

namespace MyMobiz.New
{
    public partial class Services
    {
        public Services()
        {
            Lexicons = new HashSet<Lexicons>();
            Quotes = new HashSet<Quotes>();
            Ratecategories = new HashSet<Ratecategories>();
            Servicelangs = new HashSet<Servicelangs>();
            Servicerates = new HashSet<Servicerates>();
            Webreferers = new HashSet<Webreferers>();
        }

        public string Id { get; set; }
        public string ServiceName { get; set; }
        public string ApiKey { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual ICollection<Lexicons> Lexicons { get; set; }
        public virtual ICollection<Quotes> Quotes { get; set; }
        public virtual ICollection<Ratecategories> Ratecategories { get; set; }
        public virtual ICollection<Servicelangs> Servicelangs { get; set; }
        public virtual ICollection<Servicerates> Servicerates { get; set; }
        public virtual ICollection<Webreferers> Webreferers { get; set; }
    }
}
