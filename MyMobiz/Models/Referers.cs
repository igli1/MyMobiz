using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Referers
    {
        public Referers()
        {
            Quotes = new HashSet<Quotes>();
        }

        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string Referer { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Services Service { get; set; }
        public virtual ICollection<Quotes> Quotes { get; set; }
    }
}
