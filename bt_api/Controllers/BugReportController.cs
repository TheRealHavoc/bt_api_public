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
using MimeKit;
using System.Text.RegularExpressions;

namespace bt_api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class BugReportController : ControllerBase
    {
        private readonly IBugReportRepository _bugReportRepository;
        private readonly IUserRepository _userRepository;

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<AppHub> _hubContext;

        private readonly Mail _mail;

        public BugReportController(
            IBugReportRepository bugReportRepository,
            IUserRepository userRepository,

            ApplicationDbContext context,
            IMapper mapper,
            IHubContext<AppHub> hubContext,

            Mail mail
        )
        {
            this._bugReportRepository = bugReportRepository;
            this._userRepository = userRepository;

            this._context = context;
            this._mapper = mapper;
            this._hubContext = hubContext;

            this._mail = mail;
        }

        [HttpGet]
        public async Task<ActionResult<BugReportDetailedDTO>> GetBugReport(int id)
        {
            var bugReportModel = await this._bugReportRepository.GetBugReportByID(id);

            if (bugReportModel == null)
                return NotFound();

            return Ok(this._mapper.Map<BugReportDetailedDTO>(bugReportModel));
        }

        [HttpGet]
        public async Task<ActionResult<List<BugReportDetailedDTO>>> GetBugReports()
        {
            var bugReportModels = await this._bugReportRepository.GetBugReports();

            if (bugReportModels == null)
                return NotFound();

            return Ok(this._mapper.Map<List<BugReportDetailedDTO>>(bugReportModels));
        }

        [HttpPost]
        public async Task<ActionResult<BugReportDetailedDTO>> AddBugReport([FromBody] BugReportDTO bugReportDTO)
        {
            UserModel userModel = await this._userRepository.GetUserByUsername(User.Identity.Name);

            if (userModel == null)
            {
                return BadRequest("Something went wrong.");
            }

            if (bugReportDTO.Subject.Length > 100)
                return BadRequest("Subject cannot be longer than 100 characters.");

            if (bugReportDTO.Message.Length > 600)
                return BadRequest("Subject cannot be longer than 600 characters.");

            var bugReportModel = new BugReportModel()
            {
                User = userModel,
                Subject = bugReportDTO.Subject,
                Message = bugReportDTO.Message,
            };

            await this._bugReportRepository.AddNewBugReport(bugReportModel);

            this._bugReportRepository.Save();

            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(userModel.Username, userModel.Email));
            message.Subject = "Bug Report";
            message.Body = new TextPart("plain")
            {
                Text = @$"New bug report from: {userModel.Username} ({userModel.Email}),

Subject:
{bugReportModel.Subject}

Body:
{bugReportModel.Message}"
            };

            this._mail.Send(message);

            return Ok(this._mapper.Map<BugReportDetailedDTO>(bugReportModel));
        }
    }
}
