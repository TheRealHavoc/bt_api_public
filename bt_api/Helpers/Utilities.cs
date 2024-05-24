using bt_api.Enums;
using bt_api.Models;
using System;

namespace bt_api.Helpers
{
    public static class Utilities
    {
        private static readonly Random _random = new();

        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public static int ConvertAbilityScoreToModifier(int abilityScore)
        {
            return (int) Math.Floor(
                ((double) abilityScore - 10) / 2
            );
        }
        
        public static int GetCharacterAttributeScore(CharacterModel character, AttributeEnum attribute)
        {
            switch (attribute)
            {
                case AttributeEnum.STR:
                    return character.StrengthScore;
                case AttributeEnum.DEX:
                    return character.DexterityScore;
                case AttributeEnum.CON:
                    return character.ConstitutionScore;
                case AttributeEnum.INT:
                    return 0;
                case AttributeEnum.WIS:
                    return 0;
                case AttributeEnum.CHA:
                    return 0;
                default:
                    return 0;
            }
        }

        public static int RollDice(int amount, int size)
        {
            if (amount < 1) return 1;

            var roll = 0;

            for (int i = 0; i < amount; i++)
            {
                roll = +_random.Next(1, size + 1);
            }

            return roll;
        }

        public static AttackData GenerateAttackData(CharacterModel character, AttackModel attackModel)
        {
            var d20 = RollDice(1, 20);

            var roll = d20 + ConvertAbilityScoreToModifier(
                GetCharacterAttributeScore(character, attackModel.AttackAttr)
            ) + character.ProficiencyBonus;

            return new AttackData()
            {
                Attack = attackModel,
                Roll = roll,
                IsCrit = (d20 == 20)
            };
        }

        public static DamageData GenerateDamageData(
            CharacterModel sourceCharacter,
            AttackData attackData
        )
        {
            var dieAmount = attackData.Attack.DamageDieAmount;

            if (attackData.IsCrit)
                dieAmount *= 2;

            var roll = RollDice(dieAmount, attackData.Attack.DamageDie);

            roll += ConvertAbilityScoreToModifier(
                GetCharacterAttributeScore(sourceCharacter, attackData.Attack.AttackAttr)
            );

            return new DamageData()
            {
                Damage = roll,
            };
        }
    }
}
