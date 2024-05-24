using AutoMapper;
using bt_api.DataAccessLayer;
using bt_api.DataTransferObjects;
using bt_api.Helpers;
using bt_api.Hubs;
using bt_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace bt_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IUserRepository _userRepository;

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<AppHub> _hubContext;
        public MatchController(
            IMatchRepository matchRepository,
            IUserRepository userRepository,

            ApplicationDbContext context,
            IMapper mapper,
            IHubContext<AppHub> hubContext
        ) 
        {
            this._matchRepository = matchRepository;
            this._userRepository = userRepository;

            this._context = context;
            this._mapper = mapper;
            this._hubContext = hubContext;
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<MatchDTO>> GetMatchByID(string matchID)
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
            {
                return BadRequest("Something went wrong.");
            }

            MatchModel match = await this._matchRepository.GetMatchByID(matchID);

            if (match == null)
                return NotFound();

            return Ok(this._mapper.Map<MatchDTO>(match));
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<MatchDTO>> CreateMatch()
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
            {
                return BadRequest("Something went wrong.");
            }

            if (await this._matchRepository.GetOpenMatchIDByUserID(userModel.Id) != null)
            {
                return BadRequest("You already have a match running!");
            }

            string generatedId = Utilities.GenerateRandomString(6);

            while (await this._matchRepository.GetMatchByID(generatedId) != null)
            {
                generatedId = Utilities.GenerateRandomString(6);
            }

            MatchModel newMatch = await this._matchRepository.AddNewMatch(new()
            {
                Id = generatedId,
                CreatedOn = DateTime.Now,
                MaxPlayers = 2,
            });

            await this._matchRepository.AddNewPlayerDataModel(new()
            {
                Match = newMatch,
                User = userModel,
                IsHost = true,
            });

            this._matchRepository.Save();

            await this._hubContext.Clients.User(userModel.Username).SendAsync(
                "matchCreated",
                this._mapper.Map<MatchDTO>(newMatch)
            );

            return Ok(this._mapper.Map<MatchDTO>(newMatch));
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<MatchDTO>> StartMatch(string matchId)
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
            {
                return BadRequest("Something went wrong.");
            }

            if (await this._matchRepository.GetOpenMatchIDByUserID(userModel.Id) == null)
                return BadRequest("You don't have a match running.");

            var matchModel = await this._matchRepository.GetMatchByID(matchId);

            if (matchModel == null)
                return NotFound("Could not find match with given match id.");

            if (matchModel.PlayerData.FindLast(x => x.IsHost == true).User.Id != userModel.Id)
                return Unauthorized();

            if (matchModel.EndedOn != null)
                return BadRequest("Match already ended.");

            if (matchModel.StartedOn != null)
                return BadRequest("Match already started.");

            matchModel.StartedOn = DateTime.Now;

            var turn = new TurnModel()
            {
                Match = matchModel,
                StartedOn = DateTime.Now,
                Character = this._getFirstInitiative(matchModel)
            };

            matchModel.Turns.Add(turn);

            await this._context.SaveChangesAsync();

            var userIds = matchModel.PlayerData.Select(x => x.User.Username);
            await this._hubContext.Clients.Users(userIds).SendAsync(
                "matchStarted",
                this._mapper.Map<MatchDTO>(matchModel)
            );

            return Ok(matchModel);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<MatchDTO>> EndMatch(string matchId)
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
                return BadRequest("Something went wrong.");

            var matchModel = await this._matchRepository.GetMatchByID(matchId);

            if (matchModel == null)
                return NotFound("Could not find match with given match id.");

            if (matchModel.PlayerData.SingleOrDefault(x => x.IsHost).User != userModel)
                return Unauthorized();

            if (matchModel.EndedOn != null)
                return BadRequest("Match has already ended.");

            await this._matchRepository.EndMatch(matchModel);

            this._matchRepository.Save();

            var userIds = matchModel.PlayerData.Select(x => x.User.Username);

            await this._hubContext.Clients.Users(userIds).SendAsync(
                "matchEnded"
            );

            return Ok(this._mapper.Map<MatchDTO>(matchModel));
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<MatchDTO>> LeaveMatch(string matchId)
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
                return BadRequest("Something went wrong.");

            var matchModel = await this._context.MatchDbSet
                .Where(x => x.Id == matchId)
                .Include(x => x.PlayerData)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync();

            if (matchModel == null)
                return NotFound("Could not find match with given match id.");

            if (matchModel.PlayerData.SingleOrDefault(x => x.IsHost).User == userModel)
                return BadRequest("You can't leave as the host.");

            if (matchModel.EndedOn != null)
                return BadRequest("Match has already ended.");

            matchModel.PlayerData.Remove(
                matchModel.PlayerData.SingleOrDefault(x => x.User == userModel)
            );

            this._matchRepository.Save();

            var userIds = matchModel.PlayerData.Where(x => x.User.Username != User.Identity.Name).Select(x => x.User.Username);

            await this._hubContext.Clients.Users(userIds).SendAsync(
                "playerLeft",
                userModel.Username
            );

            return Ok(this._mapper.Map<MatchDTO>(matchModel));
        }

        [HttpGet, Authorize]
        public async Task<ActionResult<MatchDTO>> GetOpenMatchByAuthenticated()
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
            {
                return BadRequest("Something went wrong.");
            }

            string runningMatchID = await this._matchRepository.GetOpenMatchIDByUserID(userModel.Id);

            if (runningMatchID == null)
                return NotFound();

            MatchModel match = await this._matchRepository.GetMatchByID(runningMatchID);

            return Ok(this._mapper.Map<MatchDTO>(match));
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<MatchDTO>> JoinMatch(string matchId)
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
            {
                return BadRequest("Something went wrong.");
            }

            if (await this._matchRepository.GetOpenMatchIDByUserID(userModel.Id) != null)
                return BadRequest("You already have a match running.");

            var matchModel = await this._matchRepository.GetMatchByID(matchId);

            if (matchModel == null)
                return NotFound("Could not find match with given match id.");

            if (matchModel.MaxPlayers == matchModel.PlayerData.Count)
                return BadRequest("Match is full.");

            if (matchModel.EndedOn != null)
                return BadRequest("Match already ended.");

            if (matchModel.StartedOn != null)
                return BadRequest("Match already started.");

            PlayerDataModel playerData = new()
            {
                Match = matchModel,
                User = userModel,
            };

            await this._matchRepository.AddNewPlayerDataModel(new()
            {
                Match = matchModel,
                User = userModel,
            });

            this._matchRepository.Save();

            var userIds = matchModel.PlayerData.Where(x => x.User.Username != User.Identity.Name).Select(x => x.User.Username);

            await this._hubContext.Clients.Users(userIds).SendAsync(
                "playerJoined",
                this._mapper.Map<PlayerDataDTO>(playerData)
            );

            return Ok(matchModel);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult<MatchDTO>> JoinRandomMatch()
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
            {
                return BadRequest("Something went wrong.");
            }

            if (await this._matchRepository.GetOpenMatchIDByUserID(userModel.Id) != null)
                return BadRequest("You already have a match running.");

            var matchID = await this._matchRepository.GetRandomMatchID();

            if (matchID == null)
                return NotFound("Could not find an open match.");

            var matchModel = await this._matchRepository.GetMatchByID(matchID);

            if (matchModel == null)
                return NotFound("Could not find an open match.");

            PlayerDataModel playerData = new()
            {
                Match = matchModel,
                User = userModel,
            };

            await this._matchRepository.AddNewPlayerDataModel(new()
            {
                Match = matchModel,
                User = userModel,
            });

            this._matchRepository.Save();

            var userIds = matchModel.PlayerData.Where(x => x.User.Username != User.Identity.Name).Select(x => x.User.Username);

            await this._hubContext.Clients.Users(userIds).SendAsync(
                "playerJoined",
                this._mapper.Map<PlayerDataDTO>(playerData)
            );

            return Ok(matchModel);
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> ToggleReady(string matchId)
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
            {
                return BadRequest("Something went wrong.");
            }

            var matchModel = await this._matchRepository.GetMatchByID(matchId);

            if (matchModel == null)
                return NotFound("Could not find match with given match id.");

            if (matchModel.EndedOn != null)
                return BadRequest("Match has already ended.");

            PlayerDataModel playerData = matchModel.PlayerData.SingleOrDefault(x => x.User == userModel);

            if (playerData == null)
                return BadRequest("You are not part of this match.");

            playerData.IsReady = !playerData.IsReady;

            this._matchRepository.Save();

            var userIds = matchModel.PlayerData.Select(x => x.User.Username);

            await this._hubContext.Clients.Users(userIds).SendAsync(
                "playerReadyToggle",
                this._mapper.Map<PlayerDataDTO>(playerData)
            );

            return Ok();
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> SetCharacter(string matchId, string characterId)
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
                return BadRequest("Something went wrong.");

            CharacterModel characterModel = await this._context.CharacterDbSet.SingleOrDefaultAsync(x => x.Id == characterId);

            if (characterModel == null)
                return NotFound("could not find character.");

            var matchModel = await this._matchRepository.GetMatchByID(matchId);

            if (matchModel == null)
                return NotFound("Could not find match with given match id.");

            if (matchModel.EndedOn != null)
                return BadRequest("Match has already ended.");

            PlayerDataModel playerData = matchModel.PlayerData.SingleOrDefault(x => x.User == userModel);

            if (playerData == null)
                return BadRequest("You are not part of this match.");

            playerData.Character = characterModel;
            playerData.CurrentHitPoints = characterModel.MaxHitPoints;

            await this._context.SaveChangesAsync();

            await this._hubContext.Clients.User(userModel.Username).SendAsync(
                "characterSelected",
                this._mapper.Map<PlayerDataDTO>(playerData)
            );

            return Ok();
        }

        [HttpPost, Authorize]
        public async Task<ActionResult> PerformAttack(string matchId, string characterId, string attackName)
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
                return BadRequest("Something went wrong.");

            CharacterModel characterModel = await this._context.CharacterDbSet.SingleOrDefaultAsync(x => x.Id == characterId);

            if (characterModel == null)
                return NotFound("could not find character.");

            var matchModel = await this._matchRepository.GetMatchByID(matchId);

            if (matchModel == null)
                return NotFound("Could not find match with given match id.");

            if (matchModel.EndedOn != null)
                return BadRequest("Match has already ended.");

            PlayerDataModel playerData = matchModel.PlayerData.SingleOrDefault(x => x.User == userModel);

            if (playerData == null)
                return BadRequest("You are not part of this match.");

            AttackModel attack = await this._context.AttackDbSet
                .SingleOrDefaultAsync(x => x.Name == attackName);

            if (attack == null)
                return BadRequest("Could not find attack.");

            TurnModel turn = matchModel.Turns.FirstOrDefault(x => x.EndedOn == null);

            if (turn == null)
                return BadRequest("No valid turn found.");

            if (turn.Character != characterModel)
                return BadRequest("It is not your turn.");

            PlayerDataModel targetPlayerData = matchModel.PlayerData.SingleOrDefault(y => y.User != userModel);

            if (targetPlayerData == null)
                return BadRequest("Could not find a target.");

            ActionModel action = new ActionModel()
            {
                Turn = turn,
                Timestamp = DateTime.Now,
            };

            // Calculate damage
            AttackData attackRoll = Utilities.GenerateAttackData(playerData.Character, attack);

            if (attackRoll.Roll >= targetPlayerData.Character.ArmorClass)
            {
                DamageData damageRoll = Utilities.GenerateDamageData(playerData.Character, attackRoll);

                targetPlayerData.CurrentHitPoints = targetPlayerData.CurrentHitPoints - damageRoll.Damage;

                action.Description = $"{playerData.Character.Name} attacks {targetPlayerData.Character.Name} with their {attack.Name} for {damageRoll.Damage} damage!";

                await this._hubContext.Clients.User(targetPlayerData.User.Username).SendAsync(
                    "playerAttacked",
                    this._mapper.Map<AttackDataDTO>(attackRoll)
                );
            } 
            else
            {
                action.Description = $"{playerData.Character.Name} attacks {targetPlayerData.Character.Name} with their {attack.Name} and misses!";
            }

            turn.EndedOn = DateTime.Now;

            var nextTurn = new TurnModel()
            {
                Match = matchModel,
                StartedOn = DateTime.Now,
                Character = targetPlayerData.Character
            };

            matchModel.Turns.Add(nextTurn);

            await this._context.AddAsync(action);
            await this._context.SaveChangesAsync();

            var userIds = matchModel.PlayerData.Select(x => x.User.Username);

            if (targetPlayerData.CurrentHitPoints <= 0 || playerData.CurrentHitPoints <= 0)
            {
                var winnerName = playerData.User.Username;

                if (playerData.CurrentHitPoints <= 0)
                {
                    winnerName = targetPlayerData.User.Username;
                }

                await this._matchRepository.EndMatch(matchModel);

                this._matchRepository.Save();

                await this._hubContext.Clients.User(winnerName).SendAsync(
                    "win"
                );

                await this._hubContext.Clients.Users(userIds).SendAsync(
                    "matchEnded"
                );
            } else
            {
                await this._hubContext.Clients.Users(userIds).SendAsync(
                    "matchUpdated",
                    this._mapper.Map<MatchDTO>(matchModel)
                );
            }

            return Ok();
        }

        private async Task<MatchModel> _getOpenMatchByUser(UserModel user)
        {
            var playerData = await this._context.PlayerDataDbSet
                .Include(x => x.Match)
                .Include(x => x.Character)
                .SingleOrDefaultAsync(x => x.User == user && x.Match.EndedOn == null);

            if (playerData == null)
                return null;

            var match = await this._context.MatchDbSet
                .Include(x => x.PlayerData)
                    .ThenInclude(x => x.User)
                .Include(x => x.PlayerData)
                    .ThenInclude(x => x.Character)
                    .ThenInclude(x => x.Attacks)
                .Include(x => x.Turns)
                    .ThenInclude(x => x.Actions)
                .SingleOrDefaultAsync(x => x.Id == playerData.Match.Id);

            return match;
        }

        private CharacterModel _getFirstInitiative(MatchModel match)
        {
            var characters = match.PlayerData.Select(x => x.Character).ToList();

            if (characters == null)
                return null;

            List<KeyValuePair<int, CharacterModel>> initiativeList = new();

            foreach (var character in characters)
            {
                var roll = Utilities.RollDice(1, 20) + character.DexterityScore;

                initiativeList.Add(new KeyValuePair<int, CharacterModel>(roll, character));
            }

            var first = initiativeList.OrderByDescending(x => x.Key).First();

            return first.Value;
        }
    }
}
