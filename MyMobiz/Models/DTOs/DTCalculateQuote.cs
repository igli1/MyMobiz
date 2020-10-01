using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using MyMobiz.Models.DTOs;
namespace MyMobiz.Models
{
    //Calculate Quote Model
    public class DTCalculateQuote
    {
        public string ServiceID { get; set; }
        public string ServiceKey { get; set; }
        public DateTime DateTimePickupTh { get; set; }
        public DateTime DateTimeArrivalTh { get; set; }
        public string Language { get; set; }
        public int Passengers { get; set; }
        public int Kms { get; set; }
        public int DriveTime { get; set; }
        public int WaitTime { get; set; }
        public List<Places> Places { get; set; }
        public List<Legs> Legs { get; set; }
        public List<DTCategories> Categories { get; set; }
        public List<DTJourney> Journey { get; set; }
    }
}
