using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace MyMobiz.Models
{
    public class DTCalculateQuote
    {
        public string ServiceID { get; set; }
        public string ServiceKey { get; set; }
       // public DateTime DateTimePickupTh { get; set; }
       // public DateTime DateTimeArrivalTh { get; set; }
        public string Language { get; set; }
        public string Veichle { get; set; }
        public Option Options { get; set; }
        public int Passengers { get; set; }
        public int Kms { get; set; }
        public int DriveTime { get; set; }
        public int WaitTime { get; set; }
        public string Places { get; set; }
        public string Legs { get; set; }
        /*public List<Placess> Places { get; internal set; }
        public void SetPlaces(List<Placess> value) => places = JsonConvert.DeserializeObject<List<Placess>>(value.ToString());
        private List<Placess> places;*/
        public class Option
        {
            bool Luxury { get; set; }
        }
    }
}
