using System.ComponentModel.DataAnnotations;

namespace bt_api.Models
{
    public class PlayerDataModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public MatchModel Match { get; set; }
        [Required]
        public UserModel User { get; set; }
        public CharacterModel Character { get; set; }
        public bool IsHost { get; set; }
        public int CurrentHitPoints { get; set; }
        public bool IsReady { get; set; }
    }
}
