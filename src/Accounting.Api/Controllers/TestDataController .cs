using Accounting.Api.DTOs.TestDataService;
using Accounting.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestDataController : ControllerBase
    {
        private readonly TestDataService _testDataService;

        public TestDataController(TestDataService testDataService)
        {
            _testDataService = testDataService;
        }

        [HttpPost]
        public async Task<ActionResult> GetStatement([FromBody] CreateTestClientsRequestDto request)
        {
            await _testDataService.CreateTestClients(request);
            return Ok();
        }
    }
}
