using System;
using System.Collections.Generic;

namespace MyMobiz.New
{
    public partial class Langs
    {
        public Langs()
        {
            Lexicons = new HashSet<Lexicons>();
            Servicelangs = new HashSet<Servicelangs>();
        }

        public string Lang { get; set; }
        public string Word { get; set; }

        public virtual ICollection<Lexicons> Lexicons { get; set; }
        public virtual ICollection<Servicelangs> Servicelangs { get; set; }
    }
}
