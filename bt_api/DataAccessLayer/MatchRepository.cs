using bt_api.Models;
using Microsoft.EntityFrameworkCore;

namespace bt_api.DataAccessLayer
{
    public class MatchRepository : IMatchRepository
    {
        private readonly ApplicationDbContext _context;

        public MatchRepository(
            ApplicationDbContext context
        ) {
            this._context = context;
        }

        public async Task<MatchModel> GetMatchByID(string matchID)
        {
            MatchModel match = await this._context.MatchDbSet
                .Where(x => x.Id == matchID)
                .Include(x => x.PlayerData)
                    .ThenInclude(x => x.User)
                .Include(x => x.PlayerData)
                    .ThenInclude(x => x.Character)
                    .ThenInclude(x => x.Attacks)
                .Include(x => x.Turns.OrderByDescending(x => x.StartedOn))
                    .ThenInclude(x => x.Actions)
                .FirstOrDefaultAsync();

            return match;
        }

        public async Task<string> GetOpenMatchIDByUserID(int userID)
        {
            var playerData = await this._context.PlayerDataDbSet
                .Include(x => x.Match)
                .Include(x => x.Character)
                .SingleOrDefaultAsync(x => x.User.Id == userID && x.Match.EndedOn == null);

            if (playerData == null)
                return null;

            return playerData.Match.Id;
        }

        public async Task<string> GetRandomMatchID()
        {
            MatchModel match = await this._context.MatchDbSet
                .Where(x => 
                    x.EndedOn == null &&
                    x.PlayerData.Count < x.MaxPlayers &&
                    x.StartedOn == null
                )
                .FirstOrDefaultAsync();

            if (match == null)
                return null;

            return match.Id;
        }

        public async Task<MatchModel> AddNewMatch(MatchModel matchModel)
        {
            await this._context.AddAsync(matchModel);

            return matchModel;
        }

        public async Task<PlayerDataModel> AddNewPlayerDataModel(PlayerDataModel playerDataModel)
        {
            await this._context.AddAsync(playerDataModel);

            return playerDataModel;
        }

        public async Task<MatchModel> EndMatch(MatchModel matchModel)
        {
            matchModel.EndedOn = DateTime.UtcNow;

            return matchModel;
        }

        public void Save()
        {
            this._context.SaveChanges();
        }
    }
}
