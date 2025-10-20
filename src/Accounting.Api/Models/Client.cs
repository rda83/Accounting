namespace Accounting.Api.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public ICollection<Account> Accounts { get; set; } = new List<Account>();
    }
}
