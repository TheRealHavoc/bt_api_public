using System.ComponentModel.DataAnnotations;

namespace bt_api.Models
{
    public class ActionModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public TurnModel Turn { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }
    }
}
