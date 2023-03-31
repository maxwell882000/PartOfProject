using Microsoft.AspNetCore.Mvc;
using VitcAuth.Repository.Interfaces;

namespace VitcAuth.Controllers
{
    [ApiController]
    [Route("apiv2/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        protected long BranchId { get => long.Parse(User.Claims.FirstOrDefault(e => e.Type == "BranchId").Value); }

        protected long AuthorId { get => long.Parse(User.Claims.FirstOrDefault(e => e.Type == "Id").Value); }
    }
}