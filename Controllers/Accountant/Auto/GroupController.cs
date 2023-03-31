using Microsoft.AspNetCore.Mvc;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using VitcAuth.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;

namespace VitcAuth.Controllers.Accountant
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class GroupController : ApiBaseController
    {

        private IGroupRepository groupRepository;
        private IMapper mapper;
        private IUserRepository userRepository;

        public GroupController(IGroupRepository groupRepository, IUserRepository userRepository, IMapper mapper)
        {
            this.groupRepository = groupRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        [HttpGet("GroupList")]
        public IActionResult GroupList(int page = 1)
        {
            // var BranchId = long.Parse(User.Claims.FirstOrDefault(e => e.Type == "BranchId").Value);
            var pagination = new Pagination<Group>(
                this.groupRepository.GetAll().Where(e => e.BranchId == BranchId), page);
            (IQueryable<Group> pagintated, int totalPage) = pagination.paginate();


            return Ok(
                new
                {
                    totalPage,
                    page,
                    data = pagintated
                    .Include(e => e.Category)
                    .Include(e => e.Students)
                    .ThenInclude(e => e.BranchStudentPayments)
                    .Include(e => e.Students)
                    .ThenInclude(e => e.Profile)
                    .OrderByDescending(e => e.StartDate)
                    .ToList()
                    .Select(e => new
                    {
                        id = e.Id,
                        group_name = e.GroupName,
                        is_debt = e.Students.Any(e =>
                        (double)(e?.Profile?.LastPrice) - e.BranchStudentPayments.AsQueryable().GetRealPrice() > 0),
                        category_name = e.Category.CategoryName,
                        active_students = e.Students.Where(e => e.SystemStatus != 1).Count(),
                        fired_students = e.Students.Where(e => e.SystemStatus == 1).Count()
                    })
                    .ToList()
                }
            );
        }

        [HttpGet("NoGroupListStudent")]
        public IActionResult NoGroupListStudent(int page = 1)
        {
            // var BranchId = int.Parse(User.Claims.FirstOrDefault(e => e.Type == "BranchId").Value);
            var pagination = new Pagination<User>(
                      this.userRepository.GetAll()
                      .Where(e => e.BranchId == BranchId && e.GroupId == null && e.RoleId == 7)
                    , page);
            (IQueryable<User> pagintated, int totalPage) = pagination.paginate();
            return Ok(new
            {
                total = totalPage,
                data = pagintated.Include(e => e.Category)
                .Include(e => e.BranchStudentPayments)
                .ThenInclude(e => e.PaymentStatus)
                .Include(e => e.Profile)
                .ToList().Select(e => this.mapper.Map<MainUser>(e)).ToList()
            });
        }


        [HttpGet("GroupList/Student/{groupId}")]
        public IActionResult GroupListStudent(long groupId)
        {

            return Ok(new
            {
                data = this.groupRepository
            .GetAll()
            .Where(e => e.Id == groupId)
            .Include(e => e.Category)
            .Include(e => e.Students)
            .ThenInclude(e => e.BranchStudentPayments)
            .ThenInclude(e => e.PaymentStatus)
            .Include(e => e.Students)
            .ThenInclude(e => e.Profile)
            .Select(e => new
            {
                branch = e.Branch.Name,
                category_name = e.Category.CategoryName,
                category_price = e.Price,
                actual_amount = e.Students.Sum(e => e.Profile.LastPrice),
                group_name = e.GroupName,
                id = e.Id,
                students = e.Students.Select(e => this.mapper.Map<MainUser>(e))
            }).First()
            });
        }
    }
}