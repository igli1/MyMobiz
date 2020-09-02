﻿using System;
using System.Collections.Generic;

namespace MyMobiz.Models
{
    public partial class Places
    {
        public Places()
        {
            LegsFromPlace = new HashSet<Legs>();
            LegsToPlace = new HashSet<Legs>();
        }

        public int Id { get; set; }
        public string Address { get; set; }
        public decimal? Lat { get; set; }
        public decimal? Lng { get; set; }

        public virtual ICollection<Legs> LegsFromPlace { get; set; }
        public virtual ICollection<Legs> LegsToPlace { get; set; }

        public static implicit operator List<object>(Places v)
        {
            throw new NotImplementedException();
        }
    }
}