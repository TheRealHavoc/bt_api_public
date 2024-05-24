using bt_api.Models;
using System.ComponentModel.DataAnnotations;

namespace bt_api.DataTransferObjects
{
    public class MatchDTO
    {
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? EndedOn { get; set; }
        public int MaxPlayers { get; set; }
        public List<PlayerDataDTO> PlayerData { get; set; }
        public List<TurnDTO> Turns { get; set; }
    }
}
