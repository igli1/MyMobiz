using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MobizAdmin.Data
{
    public class RatesDetailsModel
    {
        public RatesDetailsModel()
        {
            this.ServiceRates = new SelectList(new List<Servicerates>());
        }
        public SelectList Services { get; set; }
        public string ServiceId { get; set; }
        public SelectList ServiceRates { get; set; }
        public int? VerNum { get; set; }
    }
}
