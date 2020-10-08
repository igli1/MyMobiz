using System;

namespace MyMobiz.Models
{
    public partial class Ratetargets
    {
        public int Id { get; set; }
        public int RateDetailId { get; set; }
        public string RateTarget { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }
        public string RateOperator { get; set; }
        public decimal RateFigure { get; set; }

        public virtual Ratedetails RateDetail { get; set; }
    }
}
