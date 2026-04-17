using System;

namespace SocioManagerV2.Models
{
    public class SubscriptionPayment
    {
        public int Id { get; set; }

        public int SocioId { get; set; }
        public virtual Socio Socio { get; set; } = null!;

        public string MonthYear { get; set; } = string.Empty; // Format: "YYYY-MM"
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
    }
}
