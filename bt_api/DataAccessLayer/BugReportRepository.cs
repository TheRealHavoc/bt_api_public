using bt_api.Models;
using Microsoft.EntityFrameworkCore;

namespace bt_api.DataAccessLayer
{
    public class BugReportRepository : IBugReportRepository
    {
        private readonly ApplicationDbContext _context;

        public BugReportRepository(
            ApplicationDbContext context
        )
        {
            this._context = context;
        }

        public async Task<BugReportModel> GetBugReportByID(int id)
        {
            return await this._context.BugReportDbSet
                .Where(x => x.Id == id)
                .Include(x => x.User)
                .FirstOrDefaultAsync();
        }

        public async Task<List<BugReportModel>> GetBugReports()
        {
            return await this._context.BugReportDbSet
                .Include(x => x.User).ToListAsync();
        }

        public async Task<BugReportModel> AddNewBugReport(BugReportModel bugReportModel)
        {
            await this._context.AddAsync(bugReportModel);

            return bugReportModel;
        }

        public void Save()
        {
            this._context.SaveChanges();
        }
    }
}
