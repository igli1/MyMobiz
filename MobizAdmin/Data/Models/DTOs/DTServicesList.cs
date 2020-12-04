using System;
namespace MobizAdmin.Data
{
    public class DTServicesList
    {
        public string Id { get; set; }
        public string ServiceName { get; set; }
        public DateTime? Tsd { get; set; }
        public int CountRates { get; set; }
        public int CountCategories { get; set; }
        public int CountReferers { get; set; }
        public int CountLanguages { get; set; }
    }
}