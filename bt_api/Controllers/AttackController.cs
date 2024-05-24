using bt_api.DataAccessLayer;
using bt_api.DataTransferObjects;
using bt_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bt_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public AttackController(ApplicationDbContext context) 
        {
            this._context = context;
        }

        [HttpGet]
        public async Task<ActionResult<AttackDTO>> GetAllAttacks()
        {
            IEnumerable<AttackModel> attacks = await this._context.AttackDbSet.ToListAsync();

            return Ok(attacks);
        }
    }
}
