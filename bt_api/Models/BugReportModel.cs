using System.ComponentModel.DataAnnotations;

namespace bt_api.Models
{
    public class BugReportModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public UserModel User { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
