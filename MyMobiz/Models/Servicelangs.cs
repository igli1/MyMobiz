namespace MyMobiz.Models
{
    public partial class Servicelangs
    {
        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string Lang { get; set; }

        public virtual Langs LangNavigation { get; set; }
        public virtual Services Service { get; set; }
    }
}
