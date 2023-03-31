using Microsoft.AspNetCore.Mvc;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcAuth.Repository.Implementations;
using VitcLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;


namespace VitcAuth.Controllers.Moderator.Auto
{

    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class ModeratorStudentController : ApiBaseController
    {

        private IGroupRepository groupRepository;
        private IMapper mapper;
        private IUserRepository userRepository;
        IBranchRepository branchRepository;

        IRegionRepository regionRepository;
        IRoleRepository roleRepository;
        IUserCounterRepository userCounterRepository;

        IUserCounterRepository userCounter;

        ICategoryRepository categoryRepository;
        IBranchCategoryPriceRepository branchCategoryPrice;
        IEducationLevelTypeRepository educationLevel;
        IStudentTypeRepository studentType;
        IStudyPeriodRepository studyPeriod;
        IPreferentialTypeRepository preferentialType;
        ICountryRepository countryRepository;
        Microsoft.AspNetCore.Hosting.IHostingEnvironment environment;
        public ModeratorStudentController(
            IGroupRepository groupRepository,
            IRegionRepository regionRepository,
            IRoleRepository roleRepository,
            IUserCounterRepository userCounterRepository,
            IBranchCategoryPriceRepository branchCategoryPrice,
            IUserCounterRepository userCounter,
            ICountryRepository countryRepository,
            IBranchRepository branchRepository,
            ICategoryRepository categoryRepository,
            IUserRepository userRepository,
            IMapper mapper,
            IEducationLevelTypeRepository educationLevel,
            IStudentTypeRepository studentType,
            IStudyPeriodRepository studyPeriod,
            IPreferentialTypeRepository preferentialType,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment environment
            )
        {
            this.groupRepository = groupRepository;
            this.mapper = mapper;
            this.roleRepository = roleRepository;
            this.regionRepository = regionRepository;
            this.branchRepository = branchRepository;
            this.userCounterRepository = userCounterRepository;
            this.userRepository = userRepository;
            this.userCounter = userCounter;
            this.categoryRepository = categoryRepository;
            this.branchCategoryPrice = branchCategoryPrice;
            this.countryRepository = countryRepository;
            this.educationLevel = educationLevel;
            this.studentType = studentType;
            this.studyPeriod = studyPeriod;
            this.preferentialType = preferentialType;
            this.environment = environment;
        }

        [HttpGet("GetUserList")]
        public IActionResult GetUserList(string? search = null,
        long? GroupId = null,
        bool? IsPreferential = null,
        int page = 1)
        {

            var users = this.userRepository.GetAll()
            .Filter(search: search, GroupId: GroupId, IsPreferential: IsPreferential)
            .ListStudent()
            .Where(e => e.BranchId == BranchId);
            var pagination = new Pagination<User>(users, page, 20);
            (IQueryable<User> paginated, int totalPage) = pagination.paginate();
            return Ok(new { totalPage, data = this.mapper.Map<List<StudentAutoListResponse>>(paginated.ToList()) });
        }

        [HttpGet("ShowUser/{UserId}")]
        public IActionResult ShowUser(long UserId)
        {
            var user = this.userRepository.GetAll().ShowStudent().First(e => e.Id == UserId);
            return Ok(user);
        }
        [HttpPut("EditUser")]
        public IActionResult EditUser(UserEdit userEdit)
        {
            userEdit.SaveMedia(environment);
            this.userRepository.UpdateStudent(userEdit);
            return Ok();
        }
        [HttpPost("storeUser")]
        public IActionResult StoreUser(UserCreate user)
        {
            var branch = this.branchRepository.GetById(BranchId);
            user.BranchId = branch.Id;
            user.CompanyId = branch.CompanyId;
            user.OrganizationId = branch.OrganizationId;
            user.RegionId = branch.RegionId;
            user.SaveMedia(environment);
            this.userRepository.AddStudent(user);
            return Ok();
        }
    }
}