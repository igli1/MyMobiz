namespace MobizAdmin.Data
{
    public partial class Webreferers
    {

        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string Referer { get; set; }

        public virtual Services Service { get; set; }
    }
}
