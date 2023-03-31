using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;

namespace VitcAuth.Controllers.Accountant
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class RequeisitesController : ControllerBase
    {
        public IUserRepository userRepository;
        public IBranchRequisiteRepository branchRequisite;
        public RequeisitesController(IUserRepository userRepository, IBranchRequisiteRepository branchRequisite)
        {
            this.branchRequisite = branchRequisite;
            this.userRepository = userRepository;
        }

        [HttpGet("getRequisites")]
        public IActionResult getRequisites()
        {
            long branchId = (long)this.userRepository.currentUser(User).BranchId;
            return Ok(this.branchRequisite.GetAll().First(e => e.BranchId == branchId));
        }

        [HttpPut("editRequisites")]
        public IActionResult editGetRequisites(BranchRequisite branchRequesite)
        {
            this.branchRequisite.update(branchRequesite);
            this.branchRequisite.commit();
            return Ok(branchRequesite);
        }
    }
}