using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Ratetargets
    {
        public Ratetargets()
        {
            Ratesdetails = new HashSet<Ratesdetails>();
        }

        public string RateTarget { get; set; }

        public virtual ICollection<Ratesdetails> Ratesdetails { get; set; }
    }
}
