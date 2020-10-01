using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Ratesdetails
    {
        public int Id { get; set; }
        public int Vernum { get; set; }
        public string CategoryId { get; set; }
        public decimal RateFigure { get; set; }
        public string RateOperator { get; set; }
        public string RateTarget { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Ratecategories Category { get; set; }
        public virtual Ratetargets RateTargetNavigation { get; set; }
        public virtual Servicerates VernumNavigation { get; set; }
    }
}
