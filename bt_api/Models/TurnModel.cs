using System.ComponentModel.DataAnnotations;

namespace bt_api.Models
{
    public class TurnModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public MatchModel Match { get; set; }
        [Required]
        public CharacterModel Character { get; set; }
        [Required]
        public DateTime StartedOn { get; set; }
        public DateTime? EndedOn { get; set; } = null;
        public List<ActionModel> Actions { get; set; } = new();
    }
}