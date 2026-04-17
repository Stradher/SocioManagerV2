using System;

namespace SocioManagerV2.Models
{
    public class RoomBooking
    {
        public int Id { get; set; }

        public int RoomId { get; set; }
        public virtual Room Room { get; set; } = null!;

        public int BookerId { get; set; }
        public virtual Socio Booker { get; set; } = null!;

        public DateTime BookingDate { get; set; }
        public int DurationMinutes { get; set; }
    }
}
