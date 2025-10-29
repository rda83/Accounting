namespace Accounting.Api.DTOs.TransferService
{
    public class TransferResult
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public static TransferResult Success() => new() { IsSuccess = true };
        public static TransferResult Failed(string error) => new() { IsSuccess = false, ErrorMessage = error };
    }
}
