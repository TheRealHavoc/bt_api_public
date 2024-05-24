using bt_api.Enums;
using bt_api.Models;

namespace bt_api.DataTransferObjects
{
    public class UserDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public bool Verified { get; set; }
        public UserRoleEnum Role { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
