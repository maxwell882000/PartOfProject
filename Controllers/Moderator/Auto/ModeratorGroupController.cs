using Microsoft.AspNetCore.Mvc;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using VitcAuth.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using VitcAuth.DTO.Groups;

namespace VitcAuth.Controllers.Moderator.Auto
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class ModeratorGroupController : ApiBaseController
    {

        private IGroupRepository groupRepository;
        private IMapper mapper;
        IBranchCategoryPriceRepository branchCategoryPrice;
        private IUserRepository userRepository;
        IRoleRepository roleRepository;
        IStudyPeriodRepository studyPeriodRepository;
        IBranchRepository branchRepository;
        IUserCounterRepository userCounter;
        ICategoryRepository categoryRepository;
        IPriceRepository priceRepository;
        IGroupCurriculumRepository groupCirriculumRepository;
        IBranchStudentPaymentRepository studentPaymentRepository;
        IGroupPracticeTeacherRepository groupPracticeTeacher;
        public ModeratorGroupController(
            IGroupRepository groupRepository,
            IUserRepository userRepository,
            IBranchCategoryPriceRepository branchCategoryPrice,
            IRoleRepository roleRepository,
            IStudyPeriodRepository studyPeriodRepository,
            IBranchRepository branchRepository,
            IBranchStudentPaymentRepository studentPaymentRepository,
            IGroupPracticeTeacherRepository groupPracticeTeacher,
            IUserCounterRepository userCounter,
            IGroupCurriculumRepository groupCirriculumRepository,
            ICategoryRepository categoryRepository,
            IPriceRepository priceRepository,
            IMapper mapper)
        {
            this.groupRepository = groupRepository;
            this.branchCategoryPrice = branchCategoryPrice;
            this.mapper = mapper;
            this.roleRepository = roleRepository;
            this.branchRepository = branchRepository;
            this.studentPaymentRepository = studentPaymentRepository;
            this.studyPeriodRepository = studyPeriodRepository;
            this.userRepository = userRepository;
            this.groupCirriculumRepository = groupCirriculumRepository;
            this.categoryRepository = categoryRepository;
            this.priceRepository = priceRepository;
            this.userCounter = userCounter;
            this.groupPracticeTeacher = groupPracticeTeacher;
        }

        private CredentialResponse getCreateCredential()
        {
            var prices = this.branchCategoryPrice.GetAll().Where(e => e.BranchId == BranchId && e.Status == true)
            .Include(e => e.Category).ToList();
            var categories = prices.Select(e => e.Category).Distinct().ToList();
            var teacherId = this.roleRepository.getTeacher();
            var studyPeriod = this.studyPeriodRepository.GetAll().OrderBy(e => e.Id).ToList();
            var branches = this.branchRepository.GetAll().Where(e => e.ParentId == BranchId).ToList();
            var users = this.userRepository.GetAll()
            .Include(e => e.TeacherTypes)
            .Include(e => e.Profile)
            .Include(e => e.SystemStatusEntity)
            .Where(e => e.RoleId == teacherId
            && e.BranchId == BranchId
            && e.TeacherTypes.Count() > 0
            && (e.SystemStatusEntity == null || e.SystemStatusEntity.SystemAccess == true)
            && e.SystemStatus != 3
            ).Select(e => this.mapper.Map<UserCredResponse>(e)).ToList();

            var login = this.userCounter.GetAll().Where(e => e.Name.Contains("group")).First();
            return new CredentialResponse
            {
                Categories = categories,
                Teachers = users,
                Periods = studyPeriod,
                Login = login,
                Prices = prices,
                Branchs = branches,
            };
        }
        [HttpGet("createCredGroup")]
        public IActionResult createCredGroup()
        {
            var result = this.getCreateCredential();
            return Ok(result);
        }
        [HttpGet("updateCredGroup/{GroupId}")]
        public IActionResult updateCredGroup(long GroupId)
        {
            var result = this.getCreateCredential();
            result.Group = this.mapper.Map<GroupEditResponse>(this.groupRepository
            .GetGroup()
            .First(e => e.Id == GroupId));
            return Ok(result);
        }

        [HttpPost("StoreGroup")]
        public IActionResult StoreGroup(Group group)
        {
            var branch = this.branchRepository.GetById(BranchId);
            group.OrganizationId = (long)branch.OrganizationId;
            group.CompanyId = (long)branch.CompanyId;
            group.setVersion();
            var counter = this.userCounter.GetAll().Where(e => e.Prefix == "tr").FirstOrDefault();

            if (counter != null)
            {
                counter.Count++;
                this.userCounter.update(counter);
                this.userCounter.commit();
            }
            this.groupRepository.Add(group);
            this.groupRepository.commit();
            return Ok(new
            {
                data = this.mapper.Map<GroupDTO>(this.groupRepository.GetGroup().First(e => e.Id == group.Id))
            });
        }
        [HttpPut("UpdateGroup")]
        public async Task<IActionResult> UpdateGroup(Group group)
        {
            var old_group = this.groupRepository.GetAll()
            .Include(e => e.Practice)
            .First(e => e.Id == group.Id);
            if (old_group.Status == (int)GroupStatus.IS_NOT_ACTIVE)
            {

                if (old_group.CategoryId != group.CategoryId)
                {
                    this.groupCirriculumRepository.RemoveRange(this.groupCirriculumRepository
                    .GetAll().Where(e => e.GroupId == group.Id));
                    this.groupCirriculumRepository.commit();
                }
                var students = this.userRepository.GetAll().Where(e => e.GroupId == group.Id).ToList();
                foreach (var student in students)
                {
                    student.GroupId = null;
                    this.userRepository.update(student);
                }
                this.userRepository.commit();
            }
            old_group.setVersion();
            var user = this.userRepository.currentOnlyUser(User);
            group.OrganizationId = old_group.OrganizationId;
            group.CompanyId = old_group.CompanyId;
            this.groupPracticeTeacher.RemoveRange(old_group.Practice);
            this.groupPracticeTeacher.commit();
            this.mapper.Map<Group, Group>(group, old_group);
            this.groupRepository.update(old_group);
            this.groupRepository.commit();

            if (user.AccountantPermission || group.Status != (int)GroupStatus.IS_FINISHED)
            {
                await this.priceRepository.setAllPrices(group.Id, (double)group.Price);
            }
            else
            {
                group.Price = old_group.Price;
            }
            return Ok();
        }

        [HttpGet("GroupList")]
        public IActionResult GroupList(string? search = null, int page = 1)
        {
            var pagination = new Pagination<Group>(
                this.groupRepository.GetAll().
                Where(e => e.BranchId == BranchId)
                .Where(e => search == null || e.GroupName.ToLower().Contains(search.ToLower())), page, 15);
            (IQueryable<Group> pagintated, int totalPage) = pagination.paginate();

            return Ok(
                new
                {
                    totalPage,
                    page,
                    data = pagintated
                    .Include(e => e.Category)
                    .Include(e => e.Period)
                    .Include(e => e.MainTeacherEntity)
                    .ThenInclude(e => e.Profile)
                    .Include(e => e.Students)
                    .OrderByDescending(e => e.StartDate)
                    .AsSplitQuery()
                    .ToList()
                    .Select(e => this.mapper.Map<GroupDTO>(e))
                    .ToList()
                }
            );
        }

        [HttpGet("GetGroupOnlyList")]
        public IActionResult GetGroupOnlyList()
        {
            return Ok(new { data = this.mapper.ProjectTo<GroupOnlyResponse>(this.groupRepository.GetAll().Where(e => e.BranchId == BranchId)) });
        }

        [HttpGet("getGroupWithUser/{GroupId}")]
        public IActionResult getGroupWithUser(long GroupId)
        {
            var group = this.groupRepository.GetGroup()
            .Include(e => e.GroupActivationRequest)
            .Include(e => e.Category)
            .Include(e => e.GroupCurriculum)
            .Include(e => e.Students)
            .ThenInclude(e => e.Profile)
            .Include(e => e.Students)
            .ThenInclude(e => e.BranchStudentPayments)
            .Include(e => e.Students)
            .ThenInclude(e => e.Region)
            .ThenInclude(e => e.Parent)
            .AsSplitQuery()
            .First(e => e.Id == GroupId);
            var all_student = group.Students.ToList();
            var students = all_student.Where(e => e.SystemStatus != 1).ToList();
            var fired_students = all_student.Where(e => e.SystemStatus == 1).ToList();
            var categories = this.categoryRepository.GetAll().ToList();
            var branch_director = this.userRepository.GetAll().Where(e => e.BranchId == group.BranchId
            && e.RoleId == this.roleRepository.getDirector()).Include(e => e.Profile).First();
            return Ok(new
            {
                data = this.mapper.Map<GroupShowResponse>(group),
                students = this.mapper.Map<List<UserResponse>>(students),
                fired_students = this.mapper.Map<List<UserResponse>>(fired_students),
                categories,
                branch_director
            });
        }


    }
}