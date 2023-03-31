using VitcAuth.Repository.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VitcAuth.Controllers
{
    [Authorize]
    public class FilterKasbController : ApiBaseController
    {
        IProfessionGroupRepository groupRepository;
        IMapper mapper;
        IStudentJobRepository userRepository;
        public FilterKasbController(
            IProfessionGroupRepository groupRepository,
            IStudentJobRepository userRepository,
            IMapper mapper
        )
        {
            this.groupRepository = groupRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        [HttpGet("filterGroup")]
        public IActionResult filterGroup(long? JobId = null)
        {
            return Ok();
        }
    }
}