using System;
using System.Collections.Generic;

namespace MyMobiz.New
{
    public partial class Orders
    {
        public string Id { get; set; }
        public string QuoteId { get; set; }
        public string ClientId { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Clients Client { get; set; }
        public virtual Quotes Quote { get; set; }
    }
}
