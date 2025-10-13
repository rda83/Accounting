namespace Accounting.Api.Models
{
    public class Currency
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
