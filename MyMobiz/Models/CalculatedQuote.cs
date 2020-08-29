using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMobiz.Models
{
    public class CalculatedQuote
    {
        public Places destination{get;set;}
        public Places departure { get; set; }
        public Legs legs { get; set; }
        public Rideslegs ridesLegs { get; set; }
        public Rides rides { get; set; }
        public Quotes quotes { get; set; }
        public Services services { get; set; }
        public string language { get; set; }
        public string  options { get; set; }
        public string  veichle { get; set; }
        public int  passengers { get; set; }
    }
}
