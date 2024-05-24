namespace bt_api.DataTransferObjects
{
    public class ActionDTO
    {
        public int Id { get; set; }
        public TurnDTO Turn { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
