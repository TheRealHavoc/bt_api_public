using bt_api.Enums;
using System.ComponentModel.DataAnnotations;

namespace bt_api.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public byte[] Password { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }
        [Required]
        public string Email { get; set; }
        public bool Verified { get; set; }
        [Required]
        public UserRoleEnum Role { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenCreatedOn { get; set; }
        public DateTime RefreshTokenExpiresOn { get; set;}
    }
}
