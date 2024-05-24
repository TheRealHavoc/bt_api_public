using bt_api.Enums;
using bt_api.Models;

namespace bt_api.DataTransferObjects
{
    public class UserPublicDTO
    {
        public string Username { get; set; }
        public UserRoleEnum Role { get; set; }
    }
}
