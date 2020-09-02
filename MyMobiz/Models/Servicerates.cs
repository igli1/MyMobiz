using MyMobiz.Models;
using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Servicerates
    {
        public string ServiceId { get; set; }
        public int VerNum { get; set; }
        public DateTime DefDate { get; set; }
        public DateTime AppDate { get; set; }
        public double EurKm { get; set; }
        public double EurWaitMin { get; set; }

        public virtual Services Service { get; set; }
    }
}
