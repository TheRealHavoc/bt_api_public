using bt_api.Enums;
using System.ComponentModel.DataAnnotations;

namespace bt_api.Models
{
    public class AttackModel
    {
        [Key]
        [Required]
        public string Name { get; set; }
        [Required]
        public AttributeEnum AttackAttr { get; set; }
        [Required]
        public int DamageDieAmount { get; set; }
        [Required]
        public int DamageDie { get; set; }
        [Required]
        public AttributeEnum DamageAttr { get; set; }

        public List<CharacterModel> Characters { get; set; } = new();
    }
}
