using System.ComponentModel.DataAnnotations;

namespace bt_api.DataTransferObjects
{
    public class AttackDTO
    {
        public string Name { get; set; }
        public string AttackAttr { get; set; }
        public int DamageDieAmount { get; set; }
        public int DamageDie { get; set; }
        public string DamageAttr { get; set; }
    }
}
