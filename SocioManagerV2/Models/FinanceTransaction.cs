using System;

namespace SocioManagerV2.Models
{
    public class FinanceTransaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Concept { get; set; } = string.Empty;
        public string Type { get; set; } = "income"; // "income" or "expense"
        public string Category { get; set; } = "treasury"; // e.g. "treasury", "subscription"
    }
}
