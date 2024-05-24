using System.ComponentModel.DataAnnotations;

namespace bt_api.DataTransferObjects
{
    public class CharacterDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string AvatarURL { get; set; }
        public int ProficiencyBonus { get; set; }
        public int ArmorClass { get; set; }
        public int MaxHitPoints { get; set; }
        public int StrengthScore { get; set; }
        public int DexterityScore { get; set; }
        public int ConstitutionScore { get; set; }

        public List<AttackDTO> Attacks { get; set; } = new();
    }
}
