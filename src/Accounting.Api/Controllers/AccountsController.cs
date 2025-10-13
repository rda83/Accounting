using Accounting.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult GetStatement()
        {
            return Ok(_context.Currencies.ToList());
        }
    }
}
