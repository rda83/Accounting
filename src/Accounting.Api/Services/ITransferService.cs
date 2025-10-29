using Accounting.Api.DTOs.TransferService;

namespace Accounting.Api.Services
{
    public interface ITransferService
    {
        Task<TransferResult> TransferAsync(TransferRequest request);
    }
}
