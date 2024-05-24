namespace bt_api_tests.Builders
{
    internal class CharacterModelBuilder
    {
        private readonly List<CharacterModel> characterModels;

        public CharacterModelBuilder()
        {
            this.characterModels = new List<CharacterModel>() {
                new() {
                    Id = "CHAR01",
                    Name = "Tara Qim",
                    AvatarURL = "https://www.gmbinder.com/images/RL0wHsa.jpg",
                    ProficiencyBonus = 2,
                    ArmorClass = 14,
                    MaxHitPoints = 25,
                    StrengthScore = 12,
                    DexterityScore = 16,
                    ConstitutionScore = 12
                },
                new()
                {
                    Id = "CHAR02",
                    Name = "Marduk the Cruel",
                    AvatarURL = "https://cdnb.artstation.com/p/assets/images/images/053/718/019/large/matthaeus-milletti-kpeaze-orc-wearing-armor-style-of-dnd-character-concept-intrica-01bacfe9-4187-4495-bab0-a02498c30221.jpg?1663521038",
                    ProficiencyBonus = 2,
                    ArmorClass = 10,
                    MaxHitPoints = 32,
                    StrengthScore = 16,
                    DexterityScore = 8,
                    ConstitutionScore = 15
                }
            };
        }

        public CharacterModel GetFirstCharacter()
        {
            return this.characterModels[0];
        }

        public List<CharacterModel> GetCharacterModels()
        {
            return this.characterModels;
        }
    }
}
