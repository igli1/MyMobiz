using System;
using System.Collections.Generic;
using MyMobiz.Models.DTOs;
namespace MyMobiz.Models
{
    //Calculate Quote Model
    public class DTCalculateQuote
    {
        public string ServiceID { get; set; }
        public string ServiceKey { get; set; }
        public int VerNum { get; set; }
        public DateTime DateTimePickupTh { get; set; }
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
