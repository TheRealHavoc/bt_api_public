using bt_api.Models;

namespace bt_api.DataAccessLayer
{
    public interface IUserRepository
    {
        Task<UserModel> GetUserByUsername(string username);
        void Save();
    }
}
