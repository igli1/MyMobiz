using MobizAdmin.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MobizAdmin.Data
{
    public partial class Referers
    {
        public Referers()
        {
        }

        public int Id { get; set; }
        public string ServiceId { get; set; }
        [UrlAttribute]
        public string Referer { get; set; }
        public DateTime Tsi { get; set; }
        public DateTime Tsu { get; set; }
        [CurrentTimeValidator]
        public DateTime? Tsd { get; set; }
        public virtual Services Service { get; set; }
    }
}
