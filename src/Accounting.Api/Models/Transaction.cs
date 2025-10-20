namespace Accounting.Api.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Description { get; set; }

        public int FromAccountId { get; set; }
        public Account FromAccount { get; set; } = null!;

        public int ToAccountId { get; set; }
        public Account ToAccount { get; set; } = null!;
    }
}
