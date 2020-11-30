using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MobizAdmin.Data
{
    public class RateCategoriesType
    {
        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string Lexo { get; set; }
        public string RateGrouping { get; set; }
        public string CategoryConditions { get; set; }
        public Boolean Editable { get; set; }
        public DateTime? Tsd { get; set; }
    }
}
