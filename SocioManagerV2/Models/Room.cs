namespace SocioManagerV2.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MaxCapacity { get; set; }
    }
}
