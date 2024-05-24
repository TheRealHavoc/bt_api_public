namespace bt_api.DataTransferObjects
{
    public class PlayerDataDTO
    {
        public int Id { get; set; }
        public MatchDTO Match { get; set; }
        public UserPublicDTO User { get; set; }
        public CharacterDTO Character { get; set; }
        public bool IsHost { get; set; }
        public int CurrentHitPoints { get; set; }
        public bool IsReady { get; set; }
    }
}
