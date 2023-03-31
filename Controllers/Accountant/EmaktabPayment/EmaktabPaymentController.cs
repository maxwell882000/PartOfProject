using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitcAuth.Repository.Interfaces;

namespace VitcAuth.Controllers.Accountant
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class EmaktabController : ControllerBase
    {
        public IUserRepository userRepository;
        public IBranchRequisiteRepository branchRequisite;
        public EmaktabController(IUserRepository userRepository, IBranchRequisiteRepository branchRequisite)
        {
            this.branchRequisite = branchRequisite;
            this.userRepository = userRepository;
        }

        [HttpGet("branchPayments")]
        public IActionResult branchPayments()
        {
            return Ok();
        }
    }
}