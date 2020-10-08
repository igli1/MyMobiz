using System;
using System.Collections.Generic;

namespace MobizAdmin.Data
{
    public partial class Servicerates
    {
        public Servicerates()
        {
            Ratedetails = new HashSet<Ratedetails>();
        }

        public int VerNum { get; set; }
        public string ServiceId { get; set; }
        public DateTime DefDate { get; set; }
        public DateTime AppDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal EurKm { get; set; }
        public decimal EurMinDrive { get; set; }
        public decimal EurMinWait { get; set; }
        public decimal? EurMinimum { get; set; }
        public int MaxPax { get; set; }
        public bool Locked { get; set; }
        public string Lexo { get; set; }
        public int NQuotes { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime? Tsu { get; set; }
        public DateTime? Tsd { get; set; }

        public virtual Services Service { get; set; }
        public virtual ICollection<Ratedetails> Ratedetails { get; set; }
    }
}
