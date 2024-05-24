using bt_api.Models;

namespace bt_api.Helpers
{
    public class AttackData
    {
        public AttackModel Attack { get; set; }
        public int Roll { get; set; }
        public bool IsCrit { get; set; }
    }
}
