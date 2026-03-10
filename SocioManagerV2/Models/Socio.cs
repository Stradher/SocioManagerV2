using System;

namespace SocioManagerV2.Models
{
    public class Socio
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido1 { get; set; }
        public string? Apellido2 { get; set; }
        public int Alta { get; set; }
        public string? Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string? NumeroDeSocio { get; set; }
        public string? NumeroDeTelefono { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? DireccionPostal { get; set; }
        public DateTime FechaDeAlta { get; set; }
        public DateTime? FechaDeBaja { get; set; }
    }
}
