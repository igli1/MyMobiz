﻿using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Rategroupings
    {
        public Rategroupings()
        {
            Ratecategories = new HashSet<Ratecategories>();
        }

        public string RateGrouping { get; set; }

        public virtual ICollection<Ratecategories> Ratecategories { get; set; }
    }
}
