using Microsoft.AspNetCore.Mvc;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcAuth.Repository.Implementations;

using VitcLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using VitcAuth.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;

namespace VitcAuth.Controllers.Moderator.Kasb
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class JobModeratorStudentController : ApiBaseController
    {

        private IMapper mapper;
        private IStudentJobRepository userRepository;
        Microsoft.AspNetCore.Hosting.IHostingEnvironment environment;

        public JobModeratorStudentController(
            IStudentJobRepository userRepository,
            Microsoft.AspNetCore.Hosting.IHostingEnvironment environment,
        IMapper mapper)
        {
            this.mapper = mapper;
            this.userRepository = userRepository;
            this.environment = environment;
        }

        [HttpDelete("DeleteStudent/{UserId}")]
        public IActionResult DeleteStudent(long UserId)
        {
            this.userRepository.Remove(this.userRepository.GetById(UserId));
            this.userRepository.commit();
            return Ok();
        }

        [HttpGet("ShowEditStudent/{UserId}")]
        public IActionResult ShowEditStudent(long UserId)
        {
            return Ok(this.userRepository.GetAll()
            .Include(e => e.StudentJobPassport)
            .Include(e => e.StudentJobContacts)
            .First(e => e.Id == UserId)
            );
        }

        [HttpGet("ListStudent")]
        public IActionResult ListStudent(
            int page = 1,
            string? search = null,
            bool? IsFired = null,
            bool IsNullGroup = false)
        {
            var pagination = new Pagination<StudentJob>(this.userRepository.GetAll()
            .Filter(search: search,
            IsNullGroup: IsNullGroup,
            SystemStatus: IsFired == true ? 1 : null)
            .Include(e => e.StudentJobPassport)
            .Include(e => e.Job)
            .Include(e => e.Region)
            .Where(e => e.BranchId == BranchId), page, 20);
            (IQueryable<StudentJob> paginated, int totalPage) = pagination.paginate();
            return Ok(new { totalPage, data = paginated.ToList() });
        }

        [HttpPut("EditStudent")]
        public IActionResult EditStudent(StudentKasbCreate studentKasb)
        {
            studentKasb.SaveMedia(environment);
            var user = this.mapper.Map<StudentJob>(studentKasb);
            this.userRepository.UpdateStudent(user);
            return Ok();
        }

        [HttpPost("StoreStudent")]
        public IActionResult StoreStudent(StudentKasbCreate studentKasb)
        {
            studentKasb.SaveMedia(environment);
            var user = this.mapper.Map<StudentJob>(studentKasb);
            this.userRepository.AddStudent(user);
            return Ok();
        }
    }
}