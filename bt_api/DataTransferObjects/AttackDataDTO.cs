using bt_api.Models;

namespace bt_api.DataTransferObjects
{
    public class AttackDataDTO
    {
        public AttackDTO Attack { get; set; }
        public int Roll { get; set; }
        public bool IsCrit { get; set; }
    }
}
