using System.Globalization;
using System.Net;
using System.Web;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VitcAuth.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

using VitcAuth.Repository.Implementations;
using AutoMapper;
using VitcAuth.DTO;
using VitcAuth.Models;
using Microsoft.AspNetCore.Authorization;

namespace VitcAuth.Controllers
{

    [Authorize]
    public class FilterAutoController : ApiBaseController
    {
        IGroupRepository groupRepository;
        IMapper mapper;
        IUserRepository userRepository;
        public FilterAutoController(
            IGroupRepository groupRepository,
            IUserRepository userRepository,
            IMapper mapper
        )
        {
            this.groupRepository = groupRepository;
            this.mapper = mapper;
            this.userRepository = userRepository;
        }

        [HttpGet("filterGroup")]
        public IActionResult filterGroup(long? CategoryId = null)
        {
            var user = this.userRepository.currentOnlyUser(User);
            return Ok(
                new
                {
                    groups = this.mapper.ProjectTo<ShowTranslateModel>(this.groupRepository.GetAll().Filter(
                        CategoryId: CategoryId,
                        BranchId: BranchId,
                        StatusNot: user.AccountantPermission == false ? (int)GroupStatus.IS_FINISHED : null
                        )
                    )
                }
            );
        }

        [HttpGet("filterStudent")]
        public IActionResult filterStudent(long? GroupId = null,
        long? SystemStatusNot = null)
        {
            return Ok(
                new
                {
                    students = this.mapper.ProjectTo<ShowTranslateModel>(
                        this.userRepository.GetAll()
                                .Filter(
                                    GroupId: GroupId,
                                    SystemStatusNot: SystemStatusNot
                                )
                                .Include(e => e.Profile)

                    )
                }
            );
        }
    }
}