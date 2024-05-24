using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace bt_api.Models
{
    public class MatchModel
    {
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime? StartedOn { get; set; } = null;
        public DateTime? EndedOn { get; set; } = null;
        public int MaxPlayers { get; set; }
        public List<PlayerDataModel> PlayerData { get; set; } = new();
        public List<TurnModel> Turns { get; set; } = new();
    }
}
