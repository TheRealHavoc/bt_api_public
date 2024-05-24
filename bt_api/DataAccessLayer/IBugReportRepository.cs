using bt_api.Models;

namespace bt_api.DataAccessLayer
{
    public interface IBugReportRepository
    {
        Task<BugReportModel> GetBugReportByID(int id);
        Task<List<BugReportModel>> GetBugReports();
        Task<BugReportModel> AddNewBugReport(BugReportModel bugReportModel);

        void Save();
    }
}
