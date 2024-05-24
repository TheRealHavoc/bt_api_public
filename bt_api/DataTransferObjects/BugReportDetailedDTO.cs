namespace bt_api.DataTransferObjects
{
    public class BugReportDetailedDTO
    {
        public int Id { get; set; }
        public UserPublicDTO User { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
