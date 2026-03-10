using System;
using System.Collections.Generic;
using System.Text;

namespace SocioManagerV2.Models
{
    public class Vetado
    {
        public int id { get; set; }
        public string? dni { get; set; }
        public DateTime? fecha { get; set; }
        public string? motivo { get; set; }
    }
}
