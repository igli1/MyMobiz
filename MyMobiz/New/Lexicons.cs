using System;
using System.Collections.Generic;

namespace MyMobiz.New
{
    public partial class Lexicons
    {
        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string Lang { get; set; }
        public string Lexo { get; set; }
        public string Word { get; set; }

        public virtual Langs LangNavigation { get; set; }
        public virtual Services Service { get; set; }
    }
}
