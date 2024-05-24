using bt_api.Models;
using Microsoft.EntityFrameworkCore;

namespace bt_api.DataAccessLayer
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(
            ApplicationDbContext context
        )
        {
            _context = context;
        }

        public async Task<UserModel> GetUserByUsername(string username)
        {
            UserModel user = await this._context.UserDbSet.SingleOrDefaultAsync(x => x.Username == username);

            return user;
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
