using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocioManagerV2.Models
{
    public class BoardGameLoan
    {
        public int Id { get; set; }

        public int BoardGameId { get; set; }
        public virtual BoardGame BoardGame { get; set; } = null!;

        public int BorrowerId { get; set; }
        public virtual Socio Borrower { get; set; } = null!;

        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
