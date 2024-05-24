using System.ComponentModel.DataAnnotations;

namespace bt_api.Models
{
    public class CharacterModel
    {
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string AvatarURL { get; set; }
        [Required]
        public int ProficiencyBonus { get; set; } 
        [Required]
        public int ArmorClass { get; set; }
        [Required]
        public int MaxHitPoints { get; set; }
        [Required]
        public int StrengthScore { get; set; }
        [Required]
        public int DexterityScore { get; set; }
        [Required]
        public int ConstitutionScore { get; set; }

        public List<AttackModel> Attacks { get; set; } = new();
    }
}
