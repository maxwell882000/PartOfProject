using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitcAuth.DTO;
using VitcAuth.Repository.Interfaces;

namespace VitcAuth.Controllers.Moderator.Kasb
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class FiredKasbController : ApiBaseController
    {
        IStudentJobRepository studentJob;
        IMapper mapper;
        ISystemStatusReasonRepository systemStatusReason;
        IJobRepository jobRepository;
        public FiredKasbController(IStudentJobRepository studentJob,
         ISystemStatusReasonRepository systemStatusReason,
         IJobRepository jobRepository,
         IMapper mapper)
        {
            this.studentJob = studentJob;
            this.systemStatusReason = systemStatusReason;
            this.mapper = mapper;
            this.jobRepository = jobRepository;

        }


        [HttpGet("getFireCredential")]
        public IActionResult getFireCredential()
        {
            return Ok(new
            {
                reasons = this.mapper.Map<List<ShowTranslateModel>>(this.systemStatusReason
                .GetAll().Where(e => e.ForStudent == true).ToList()),
                categories = this.mapper.Map<List<ShowTranslateModel>>(this.jobRepository.GetAll()
                )
            });
        }
    }
}