using VitcAuth.Repository.Interfaces;
using VitcAuth.Repository.Implementations;
using Microsoft.EntityFrameworkCore;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitcAuth.DTO;
using VitcAuth.Models;
using VitcLibrary.Helpers;

namespace VitcAuth.Controllers.Moderator.Auto
{

    [Authorize]
    public class FiredAutoController : ApiBaseController
    {
        private IMapper mapper;
        private IUserRepository userRepository;
        private IBranchCategoryPriceRepository categoryRepository;
        ISystemStatusReasonRepository statusReasonRepository;
        IUserProfileRepository profileRepository;
        ISystemStatusStepRepository statusStepRepository;
        public FiredAutoController(IMapper mapper,
        IUserRepository userRepository,
       IBranchCategoryPriceRepository categoryRepository,
       ISystemStatusReasonRepository statusReasonRepository,
       ISystemStatusStepRepository statusStepRepository,
       IUserProfileRepository profileRepository)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.categoryRepository = categoryRepository;
            this.profileRepository = profileRepository;
            this.statusReasonRepository = statusReasonRepository;
            this.statusStepRepository = statusStepRepository;
        }

        [HttpPost("FireStudent")]
        public IActionResult CreateFireStudent(SystemStatusStep statusStep)
        {
            this.statusStepRepository.FireStudent(statusStep);
            return Ok(this.userRepository.GetAll().GetFiredStudent().First(e => e.Id == statusStep.UserId));
        }

        [HttpPut("FireStudent")]
        public IActionResult UpdateFireStudent(SystemStatusStep statusStep)
        {
            this.statusStepRepository.UpdateFireStudent(statusStep);
            return Ok();
        }

        [HttpGet("CalculateFirePrice")]
        public IActionResult CalculateFirePrice(long StudentId, DateOnly DeductDate)
        {
            var user = this.userRepository.GetAll().Include(e => e.Group).First(e => e.Id == StudentId);
            var profile = this.profileRepository.calculateLostPrice(
                    user, DeductDate);
            var response = this.mapper.Map<CalculatePriceResponse>(profile);
            response.StartDate = user.Group.StartDate;
            response.EndDate = user.Group.EndDate;
            return Ok(response);
        }

        [HttpGet("getListFiredStudents")]
        public IActionResult getListFiredStudents(int page = 1)
        {
            var pagination = new Pagination<User>(this.userRepository.GetAll()
            .Where(e => e.BranchId == BranchId).GetFiredStudent(), page, 10);
            (IQueryable<User> paginated, int totalPage) = pagination.paginate();
            var users = this.mapper.Map<List<StudentFireResponse>>(paginated.ToList());
            return Ok(new { totalPage, data = users });
        }

        [HttpGet("getFireCredential")]
        public IActionResult getFireCredential()
        {
            return Ok(new
            {
                reasons = this.mapper.Map<List<ShowTranslateModel>>(this.statusReasonRepository
                .GetAll().Where(e => e.ForStudent == true).ToList()),
                categories = this.mapper.Map<List<ShowTranslateModel>>(this.categoryRepository.GetAll()
                .Categories(e => e.BranchId == BranchId))
            });
        }
    }
}