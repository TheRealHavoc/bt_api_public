namespace bt_api.DataTransferObjects
{
    public class TurnDTO
    {
        public int Id { get; set; }
        public MatchDTO Match { get; set; }
        public CharacterDTO Character { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime? EndedOn { get; set; } = null;
        public List<ActionDTO> Actions { get; set; } = new();
    }
}
