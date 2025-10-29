using Accounting.Api.DTOs.TransferService;
using Accounting.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpPost]
        public async Task<IActionResult> Transfer([FromBody] TransferRequest request)
        {
            var result = await _transferService.TransferAsync(request);

            if (result.IsSuccess)
            {
                return Ok(new { message = "Перевод выполнен успешно" });
            }

            return BadRequest(new { error = result.ErrorMessage });
        }
    }
}
