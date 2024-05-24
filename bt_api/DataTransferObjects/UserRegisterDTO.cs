using bt_api.Models;
using System.ComponentModel.DataAnnotations;

namespace bt_api.DataTransferObjects
{
    public class UserRegisterDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
    }
}
