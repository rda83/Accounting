using Accounting.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            return Ok(_context.Accounts.ToList());
        }

        //[HttpGet]
        //public ActionResult GetStatement()
        //{
        //    return Ok(_context.Currencies.ToList());
        //}


        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Currency>>> GetStatement()
        //{
        //    var currencies = await _context.Currencies
        //        .AsNoTracking()
        //        .ToListAsync();

        //    return Ok(currencies);
        //}


    }
}
