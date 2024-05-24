using AutoMapper;
using bt_api.DataTransferObjects;
using bt_api.Models;

namespace bt_api.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() {
            CreateMap<MatchModel, MatchDTO>();
            CreateMap<UserModel, UserDTO>();
            CreateMap<UserModel, UserPublicDTO>();
            CreateMap<CharacterModel, CharacterDTO>();
            CreateMap<PlayerDataModel, PlayerDataDTO>();
            CreateMap<AttackModel, AttackDTO>();
            CreateMap<TurnModel, TurnDTO>();
            CreateMap<ActionModel, ActionDTO>();
            CreateMap<AttackData, AttackDataDTO>();
            CreateMap<BugReportModel, BugReportDTO>();
            CreateMap<BugReportModel, BugReportDetailedDTO>();
        }
    }
}
