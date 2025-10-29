namespace Accounting.Api.DTOs.TransferService
{
    public class TransferRequest
    {
        public string FromAccountNumber { get; set; } = null!;
        public string ToAccountNumber { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
    }
}