using System.ComponentModel.DataAnnotations.Schema;

namespace SocioManagerV2.Models
{
    public class BoardGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? OwnerId { get; set; }
        
        public virtual Socio? Owner { get; set; }
    }
}
