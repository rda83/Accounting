namespace Accounting.Api.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Number { get; set; } = null!;
        public decimal Balance { get; set; }
        public int CurrencyId { get; set; }
        public Currency Currency { get; set; } = null!;
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public ICollection<Transaction> DebitTransactions { get; set; } = new List<Transaction>();
        public ICollection<Transaction> CreditTransactions { get; set; } = new List<Transaction>();
    }
}
