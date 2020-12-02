namespace MobizAdmin.Data
{
    public class DTServiceLanguages
    {
        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string Lang { get; set; }
        public string Word { get; set; }
        public bool Existing { get; set; }
    }
}
