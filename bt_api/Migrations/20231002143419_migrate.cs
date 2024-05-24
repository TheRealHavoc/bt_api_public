using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bt_api.Migrations
{
    public partial class migrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttackDbSet",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AttackAttr = table.Column<int>(type: "int", nullable: false),
                    DamageDieAmount = table.Column<int>(type: "int", nullable: false),
                    DamageDie = table.Column<int>(type: "int", nullable: false),
                    DamageAttr = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttackDbSet", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "CharacterDbSet",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProficiencyBonus = table.Column<int>(type: "int", nullable: false),
                    ArmorClass = table.Column<int>(type: "int", nullable: false),
                    MaxHitPoints = table.Column<int>(type: "int", nullable: false),
                    StrengthScore = table.Column<int>(type: "int", nullable: false),
                    DexterityScore = table.Column<int>(type: "int", nullable: false),
                    ConstitutionScore = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterDbSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatchDbSet",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MaxPlayers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchDbSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Verified = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenCreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshTokenExpiresOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDbSet", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterAttack",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AttackName = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterAttack", x => new { x.CharacterId, x.AttackName });
                    table.ForeignKey(
                        name: "FK_CharacterAttack_AttackDbSet_AttackName",
                        column: x => x.AttackName,
                        principalTable: "AttackDbSet",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterAttack_CharacterDbSet_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "CharacterDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TurnDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CharacterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TurnDbSet_CharacterDbSet_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "CharacterDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TurnDbSet_MatchDbSet_MatchId",
                        column: x => x.MatchId,
                        principalTable: "MatchDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlayerDataDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CharacterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    IsHost = table.Column<bool>(type: "bit", nullable: false),
                    CurrentHitPoints = table.Column<int>(type: "int", nullable: false),
                    IsReady = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerDataDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayerDataDbSet_CharacterDbSet_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "CharacterDbSet",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PlayerDataDbSet_MatchDbSet_MatchId",
                        column: x => x.MatchId,
                        principalTable: "MatchDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerDataDbSet_UserDbSet_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActionDbSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TurnId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionDbSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActionDbSet_TurnDbSet_TurnId",
                        column: x => x.TurnId,
                        principalTable: "TurnDbSet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AttackDbSet",
                columns: new[] { "Name", "AttackAttr", "DamageAttr", "DamageDie", "DamageDieAmount" },
                values: new object[,]
                {
                    { "Axe", 1, 1, 6, 2 },
                    { "Longsword", 0, 0, 8, 1 },
                    { "Rapier", 1, 1, 8, 1 },
                    { "Shortbow", 1, 1, 6, 1 }
                });

            migrationBuilder.InsertData(
                table: "CharacterDbSet",
                columns: new[] { "Id", "ArmorClass", "AvatarURL", "ConstitutionScore", "DexterityScore", "MaxHitPoints", "Name", "ProficiencyBonus", "StrengthScore" },
                values: new object[,]
                {
                    { "CHAR01", 13, "https://www.gmbinder.com/images/RL0wHsa.jpg", 12, 14, 26, "Tara Qim", 2, 14 },
                    { "CHAR02", 10, "https://cdnb.artstation.com/p/assets/images/images/053/718/019/large/matthaeus-milletti-kpeaze-orc-wearing-armor-style-of-dnd-character-concept-intrica-01bacfe9-4187-4495-bab0-a02498c30221.jpg?1663521038", 15, 8, 32, "Marduk the Cruel", 2, 16 },
                    { "CHAR03", 15, "https://i.pinimg.com/736x/1b/6c/e9/1b6ce97153d3f7bbed330e3f7dc02293.jpg", 12, 16, 26, "Stella", 2, 12 }
                });

            migrationBuilder.InsertData(
                table: "CharacterAttack",
                columns: new[] { "AttackName", "CharacterId" },
                values: new object[,]
                {
                    { "Longsword", "CHAR01" },
                    { "Axe", "CHAR02" },
                    { "Rapier", "CHAR03" },
                    { "Shortbow", "CHAR03" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActionDbSet_TurnId",
                table: "ActionDbSet",
                column: "TurnId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterAttack_AttackName",
                table: "CharacterAttack",
                column: "AttackName");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDataDbSet_CharacterId",
                table: "PlayerDataDbSet",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDataDbSet_MatchId",
                table: "PlayerDataDbSet",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerDataDbSet_UserId",
                table: "PlayerDataDbSet",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnDbSet_CharacterId",
                table: "TurnDbSet",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_TurnDbSet_MatchId",
                table: "TurnDbSet",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDbSet_Username",
                table: "UserDbSet",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActionDbSet");

            migrationBuilder.DropTable(
                name: "CharacterAttack");

            migrationBuilder.DropTable(
                name: "PlayerDataDbSet");

            migrationBuilder.DropTable(
                name: "TurnDbSet");

            migrationBuilder.DropTable(
                name: "AttackDbSet");

            migrationBuilder.DropTable(
                name: "UserDbSet");

            migrationBuilder.DropTable(
                name: "CharacterDbSet");

            migrationBuilder.DropTable(
                name: "MatchDbSet");
        }
    }
}
