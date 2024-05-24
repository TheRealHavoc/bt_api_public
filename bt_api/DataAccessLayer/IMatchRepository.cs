using bt_api.Models;

namespace bt_api.DataAccessLayer
{
    public interface IMatchRepository
    {
        Task<MatchModel> GetMatchByID(string matchID);
        Task<string> GetOpenMatchIDByUserID(int userID);
        Task<string> GetRandomMatchID();

        Task<MatchModel> AddNewMatch(MatchModel matchModel);
        Task<PlayerDataModel> AddNewPlayerDataModel(PlayerDataModel playerDataModel);
        Task<MatchModel> EndMatch(MatchModel matchModel);

        void Save();
    }
}
