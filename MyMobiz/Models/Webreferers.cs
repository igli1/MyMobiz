using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Webreferers
    {
        public Webreferers()
        {
            Quotes = new HashSet<Quotes>();
        }

        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string Referer { get; set; }

        public virtual Services Service { get; set; }
        public virtual ICollection<Quotes> Quotes { get; set; }
    }
}
