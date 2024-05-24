using AutoMapper;
using bt_api.DataAccessLayer;
using bt_api.DataTransferObjects;
using bt_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace bt_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CharacterController(ApplicationDbContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterDTO>>> GetCharacters()
        {
            IEnumerable<CharacterModel> characterList = await this._context.CharacterDbSet
                .Include(x => x.Attacks)
                .ToListAsync();

            return Ok(this._mapper.Map<IEnumerable<CharacterModel>>(characterList));
        }

        [HttpGet]
        public async Task<ActionResult<CharacterDTO>> GetCharacter(string characterId)
        {
            CharacterModel character = await this._context.CharacterDbSet
                .Where(x => x.Id == characterId)
                .Include(x => x.Attacks)
                .SingleOrDefaultAsync();

            return Ok(this._mapper.Map<CharacterModel>(character));
        }
    }
}
