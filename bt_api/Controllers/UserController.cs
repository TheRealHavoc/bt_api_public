using bt_api.DataAccessLayer;
using bt_api.DataTransferObjects;
using bt_api.Enums;
using bt_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Unicode;

namespace bt_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public UserController(IConfiguration configuration, ApplicationDbContext context)
        {
            this._configuration = configuration;
            this._context = context;
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Register([FromBody] UserRegisterDTO userRegisterDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await this._context.UserDbSet.AnyAsync(x => x.Username.ToLower() == userRegisterDTO.Username.ToLower()))
                return BadRequest("A user with this name already exists.");

            this.CreatePasswordHash(userRegisterDTO.Password, out byte[] passwordHash, out byte[] passwordSalt);

            UserModel user = new UserModel
            {
                Username = userRegisterDTO.Username,
                Password = passwordHash,
                PasswordSalt = passwordSalt,
                Email = userRegisterDTO.Email,
                Verified = false,
                Role = Enums.UserRoleEnum.Player,
            };

            await this._context.AddAsync(user);
            await this._context.SaveChangesAsync();

            UserDTO userDTO = new UserDTO
            {
                Username = user.Username,
                Email = user.Email,
                Verified = user.Verified,
                Role = user.Role,
            };

            return Ok(userDTO);
        }

        [HttpPost]
        public async Task<ActionResult<UserDTO>> Login([FromBody] UserLoginDTO userLoginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserModel userModel = await this._context.UserDbSet.SingleOrDefaultAsync(x => x.Username.ToLower() == userLoginDTO.Username.ToLower());

            if (userModel == null || !this.VerifyPasswordHash(userLoginDTO.Password, userModel.Password, userModel.PasswordSalt))
                return BadRequest("Invalid credentials provided.");

            RefreshToken refreshToken = this.GenerateRefreshToken();

            userModel.RefreshToken = refreshToken.Token;
            userModel.RefreshTokenCreatedOn = refreshToken.Created;
            userModel.RefreshTokenExpiresOn = refreshToken.Expires;

            await this._context.SaveChangesAsync();

            UserDTO userDTO = new UserDTO
            {
                Username = userModel.Username,
                Email = userModel.Email,
                Verified = userModel.Verified,
                Role = userModel.Role,
                Token = CreateToken(userModel),
                RefreshToken = refreshToken.Token
            };

            return Ok(userDTO);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUserByRefreshToken([FromBody] string refreshToken)
        {
            UserModel userModel = await this._context.UserDbSet.SingleOrDefaultAsync(x => x.RefreshToken == refreshToken);

            if (userModel == null)
                return Unauthorized("Invalid refresh token provided.");

            if (userModel.RefreshTokenExpiresOn < DateTime.Now)
                return Unauthorized("Session has expired.");

            RefreshToken newRefreshToken = this.GenerateRefreshToken();

            userModel.RefreshToken = newRefreshToken.Token;
            userModel.RefreshTokenCreatedOn = newRefreshToken.Created;
            userModel.RefreshTokenExpiresOn = newRefreshToken.Expires;

            await this._context.SaveChangesAsync();

            UserDTO userDTO = new UserDTO
            {
                Username = userModel.Username,
                Email = userModel.Email,
                Verified = userModel.Verified,
                Role = userModel.Role,
                Token = CreateToken(userModel),
                RefreshToken = newRefreshToken.Token
            };

            return Ok(userDTO);
        }

        [HttpGet, Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            IEnumerable<UserModel> userModels = await this._context.UserDbSet.ToListAsync();

            return Ok(userModels);
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<IEnumerable<UserRoleEnum>>> GetUserRoles()
        {
            return Enum.GetValues(typeof(UserRoleEnum)).Cast<UserRoleEnum>().ToList();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(UserModel userModel)
        {
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.Name, userModel.Username),
                new Claim(ClaimTypes.NameIdentifier, userModel.Username),
                new Claim(ClaimTypes.Email, userModel.Email),
                new Claim(ClaimTypes.Role, userModel.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Jwt:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }
    }
}
