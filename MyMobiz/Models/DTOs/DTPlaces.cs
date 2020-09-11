using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMobiz.Models.DTOs
{
    public class DTPlaces
    {
        public string JsId { get; set; }
        public string Value { get; set; }
        public string Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }
    }
}
