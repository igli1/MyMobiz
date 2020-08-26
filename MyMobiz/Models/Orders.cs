using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Orders
    {
        public string Id { get; set; }
        public string QuoteId { get; set; }
        public string ClientId { get; set; }

        public virtual Clients Client { get; set; }
        public virtual Quotes Quote { get; set; }
    }
}
