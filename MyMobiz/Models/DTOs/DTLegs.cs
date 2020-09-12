using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMobiz.Models.DTOs
{
    public class DTLegs
    {
        
        public int? Kms { get; set; }
        public int? MinutesDrive { get; set; }
        public int? MinutesWithTraffic { get; set; }
        public double? Fare { get; set; }
        public FromGEO fromGEO { get; set; }
        public ToGEO toGEO { get; set; }
        public class FromGEO
        {
            public string JsId { get; set; }
        }
        public class ToGEO
        {
            public string JsId { get; set; }
        }
    }
}
