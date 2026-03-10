using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace SocioManagerV2.Models
{
    internal class Usuario
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string email { get; set; }
        public string nombre { get; set; }
        public string apellidos { get; set; }
        public string passwordHash { get; set; }
        public bool isAdmin { get; set; }
    }
}
