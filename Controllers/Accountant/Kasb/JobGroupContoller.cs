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
    public class JobGroupController : ApiBaseController
    {

        private IJobGroupRepository groupRepository;
        private IMapper mapper;
        private IStudentJobRepository studentJob;
        public JobGroupController(IJobGroupRepository groupRepository, IStudentJobRepository studentJob, IMapper mapper)
        {
            this.groupRepository = groupRepository;
            this.mapper = mapper;
            this.studentJob = studentJob;
        }

        [HttpGet("GroupList")]
        public IActionResult GroupList(int page = 1)
        {
            var pagination = new Pagination<ProfessionGroup>(
                this.groupRepository.GetAll().Where(e => e.BranchId == BranchId), page);
            (IQueryable<ProfessionGroup> pagintated, int totalPage) = pagination.paginate();
            return Ok(
                new
                {
                    totalPage,
                    data = pagintated
                    .Include(e => e.Students)
                    .ThenInclude(e => e.Payment)
                    .Include(e => e.Job)
                    .OrderByDescending(e => e.StartDate)
                    .ToList()
                    .Select(e => new
                    {
                        id = e.Id,
                        group_name = e.GroupName,
                        actual_amount = e.Students.Where(e => e.SystemStatus != 1).Sum(e => e.EducationPrice),
                        paid_amount = e.Students.Select(e =>
                        new
                        {
                            price = e.EducationPrice - e.Payment.AsQueryable().GetRealPrice()
                        }
                        ).First().price,
                        job_name = e.Job.NameLat,
                        students = e.Students.Where(e => e.SystemStatus != 1).Count()
                    }).ToList()
                }
            );
        }
        [HttpGet("NoGroupListStudent")]
        public IActionResult NoGroupListStudent(int page = 1)
        {
            // var BranchId = int.Parse(User.Claims.FirstOrDefault(e => e.Type == "BranchId").Value);
            var pagination = new Pagination<StudentJob>(
                      this.studentJob.GetAll().Where(e => e.BranchId == BranchId), page);
            (IQueryable<StudentJob> pagintated, int totalPage) = pagination.paginate();
            return Ok(new
            {
                total = totalPage,
                data = pagintated
                .Include(e => e.Payment)
                .ThenInclude(e => e.Statuses)
                .ToList()
            });
        }


        [HttpGet("GroupList/Student/{groupId}")]
        public IActionResult GroupListStudent(long groupId) => Ok(new
        {
            data = this.groupRepository
            .GetAll()
            .Where(e => e.Id == groupId)
            .Include(e => e.Job)
            .Include(e => e.Students)
            .ThenInclude(e => e.Payment)
            .ThenInclude(e => e.Statuses)
            .Select(e => new
            {
                actual_amount = e.Students.Sum(e => e.EducationPrice),
                branch = e.Branch.Name,
                category_name = e.Job.Name,
                category_price = e.Price,
                group_name = e.GroupName,

                paid_amount = e.Students.Select(e =>
                      new
                      {
                          price = e.EducationPrice - e.Payment.AsQueryable().GetRealPrice()
                      }
                        ).First().price,
                id = e.Id,
                students = e.Students
            }).First()
        });
    }
}