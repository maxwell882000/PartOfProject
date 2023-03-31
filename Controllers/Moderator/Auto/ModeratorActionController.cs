using Microsoft.AspNetCore.Mvc;
using VitcAuth.Repository.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace VitcAuth.Controllers.Moderator.Auto
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class ModeratorActionController : ApiBaseController
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
        public ModeratorActionController(
            IGroupRepository groupRepository,
            IUserRepository userRepository,
            IBranchCategoryPriceRepository branchCategoryPrice,
            IRoleRepository roleRepository,
            IStudyPeriodRepository studyPeriodRepository,
            IBranchRepository branchRepository,
            IBranchStudentPaymentRepository studentPaymentRepository,
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
        }

        [HttpGet("StudentToGroup")]
        public IActionResult StudentToGroup(long CategoryId, long GroupId)
        {
            var group = this.groupRepository.GetAll()
            .Include(e => e.Students)
            .First(e => e.Id == GroupId);
            var students = this.userRepository.GetAll()
            .Include(e => e.Profile)
            .Include(e => e.Region)
            .ThenInclude(e => e.Parent)
            .Where(e => e.Period == group.PeriodId
            && e.CategoryId == CategoryId && e.GroupId == null
            && e.BranchId == BranchId)
            .ToList();
            var countStudent = group.Students.Count();

            return Ok(new { data = students, StudentInGroup = countStudent, GroupName = group.GroupName });
        }

        [HttpPost("StudentToGroup")]
        public IActionResult AttachStudentToGroup(AttachStudent attach)
        {
            var users = this.userRepository.GetAll().Include(e => e.BranchStudentPayments).Where(e => attach.StudentId.Contains(e.Id)).ToList();
            var group = this.groupRepository.GetAll().Include(e => e.Students).First(e => e.Id == attach.GroupId);

            if (group.Students.Count() + attach.StudentId.Count() <= 25)
            {
                foreach (var user in users)
                {
                    user.GroupId = attach.GroupId;
                    foreach (var payment in user.BranchStudentPayments)
                    {
                        payment.GroupId = attach.GroupId;
                    }
                    this.studentPaymentRepository.AddRange(user.BranchStudentPayments);
                }
                this.userRepository.AddRange(users);
                this.userRepository.commit();
                this.priceRepository.setAllPrices(attach.GroupId, group.Price ?? 0);
                return Ok();
            }
            this.ModelState.AddModelError("status", "error");
            return ValidationProblem();
        }

        [HttpDelete("StudentToGroup/{StudentId}")]
        public IActionResult DeleteStudentToGroup(long StudentId)
        {
            var student = this.userRepository.GetById(StudentId);
            student.GroupId = null;
            this.userRepository.update(student);
            this.userRepository.commit();
            return Ok(new { status = "Ok" });
        }


        [HttpGet("ActiveteGroup/{GroupId}")]
        public IActionResult ActiveteGroup(long GroupId)
        {
            var group = this.groupRepository.GetById(GroupId);
            group.Status = 1;
            this.groupRepository.update(group);
            this.groupRepository.commit();
            return Ok();
        }

        [HttpDelete("RemoveGroup/{GroupId}")]
        public IActionResult RemoveGroup(long GroupId)
        {
            this.groupRepository.Remove(this.groupRepository.GetById(GroupId));
            this.groupRepository.commit();
            return Ok();
        }
    }
}