using bt_api.Enums;
using bt_api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;

namespace bt_api.DataAccessLayer
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<ActionModel> ActionDbSet { get; set; }
        public DbSet<AttackModel> AttackDbSet { get; set; }
        public DbSet<CharacterModel> CharacterDbSet { get; set; }
        public DbSet<MatchModel> MatchDbSet { get; set; }
        public DbSet<PlayerDataModel> PlayerDataDbSet { get; set; }
        public DbSet<TurnModel> TurnDbSet { get; set; }
        public DbSet<UserModel> UserDbSet { get; set; }
        public DbSet<BugReportModel> BugReportDbSet { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>().HasIndex(u => u.Username).IsUnique();

            modelBuilder.Entity<CharacterModel>()
                .HasMany(e => e.Attacks)
                .WithMany(e => e.Characters)
                .UsingEntity(
                    "CharacterAttack",
                    l => l.HasOne(typeof(AttackModel)).WithMany().HasForeignKey("AttackName").HasPrincipalKey(nameof(AttackModel.Name)),
                    r => r.HasOne(typeof(CharacterModel)).WithMany().HasForeignKey("CharacterId").HasPrincipalKey(nameof(CharacterModel.Id)),
                    j => {
                        j.HasKey("CharacterId", "AttackName");
                        j.HasData(
                            new { CharacterId = "CHAR01", AttackName = "Longsword" },
                            new { CharacterId = "CHAR02", AttackName = "Axe" },
                            new { CharacterId = "CHAR03", AttackName = "Rapier" },
                            new { CharacterId = "CHAR03", AttackName = "Shortbow" }
                        );
                    }
                );


            var longswordAttack = new AttackModel
            {
                Name = "Longsword",
                AttackAttr = AttributeEnum.STR,
                DamageDieAmount = 1,
                DamageDie = 8,
                DamageAttr = AttributeEnum.STR,
            };

            var axeAttack = new AttackModel
            {
                Name = "Axe",
                AttackAttr = AttributeEnum.DEX,
                DamageDieAmount = 2,
                DamageDie = 6,
                DamageAttr = AttributeEnum.DEX,
            };

            var rapierAttack = new AttackModel
            {
                Name = "Rapier",
                AttackAttr = AttributeEnum.DEX,
                DamageDieAmount = 1,
                DamageDie = 8,
                DamageAttr = AttributeEnum.DEX,
            };

            var shortBowAttack = new AttackModel
            {
                Name = "Shortbow",
                AttackAttr = AttributeEnum.DEX,
                DamageDieAmount = 1,
                DamageDie = 6,
                DamageAttr = AttributeEnum.DEX,
            };

            modelBuilder.Entity<AttackModel>().HasData(longswordAttack);
            modelBuilder.Entity<AttackModel>().HasData(axeAttack);
            modelBuilder.Entity<AttackModel>().HasData(rapierAttack);
            modelBuilder.Entity<AttackModel>().HasData(shortBowAttack);

            modelBuilder.Entity<CharacterModel>().HasData(new CharacterModel
            {
                Id = "CHAR01",
                Name = "Tara Qim",
                AvatarURL = "https://www.gmbinder.com/images/RL0wHsa.jpg",
                ProficiencyBonus = 2,
                ArmorClass = 13,
                MaxHitPoints = 26,
                StrengthScore = 14,
                DexterityScore = 14,
                ConstitutionScore = 12
            });

            modelBuilder.Entity<CharacterModel>().HasData(new CharacterModel
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
            });

            modelBuilder.Entity<CharacterModel>().HasData(new CharacterModel
            {
                Id = "CHAR03",
                Name = "Stella",
                AvatarURL = "https://i.pinimg.com/736x/1b/6c/e9/1b6ce97153d3f7bbed330e3f7dc02293.jpg",
                ProficiencyBonus = 2,
                ArmorClass = 15,
                MaxHitPoints = 26,
                StrengthScore = 12,
                DexterityScore = 16,
                ConstitutionScore = 12
            });
        }
    }
}
