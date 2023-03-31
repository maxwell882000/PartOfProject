using Microsoft.AspNetCore.Mvc;
using VitcAuth.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using VitcAuth.DTO.Payments;
using VitcAuth.Models;
using Microsoft.AspNetCore.Authorization;

namespace VitcAuth.Controllers.Accountant
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class ChangePriceController : ControllerBase
    {

        private IUserProfileRepository userProfileRepository;
        private IPriceRepository priceRepository;
        private IUserRepository userRepository;
        public ChangePriceController(IUserProfileRepository profileRepository,
         IUserRepository userRepository,
         IPriceRepository priceRepository)
        {
            this.userProfileRepository = profileRepository;
            this.priceRepository = priceRepository;
            this.userRepository = userRepository;
        }

        private void checkOnConditions(User user, ChangeCategoryPrice change)
        {
            var currentUser = this.userRepository.currentUser(User);
            if (
                         ((
                                 (user.SystemStatus == 1 || user.StudentTypeId == 5)
                                 && change.AddPrice != null
                             ) || (
                                 (user.SystemStatus == 1 || (user.StudentTypeId == 5 && user.Preferential == 0))
                                 && change.AddPrice == null)) && currentUser.AccountantPermission && user.CategoryId != 10)
            {
                this.ModelState.AddModelError("data", "cannotremove");
                throw new Exception();
            }
        }
        
        private void setProfileDataReduce(UserProfile profile, string? reason, string? decreeNumber, DateOnly? date)
        {
            profile.ReduceReason = reason;
            profile.ReduceDecreeNumber = decreeNumber;
            profile.ReduceDate = date;
        }

        private void setProfileDataAdd(UserProfile profile, string? reason, string? decreeNumber, DateOnly? date)
        {
            profile.AddReason = reason;
            profile.AddDecreeNumber = decreeNumber;
            profile.AddDate = date;
        }

        [HttpPost("changeCategoryPrice")]
        public async Task<IActionResult> changeCategoryPrice(ChangeCategoryPrice change)
        {
            var user = this.userRepository.GetAll()
            .Include(e => e.Profile).First(e => e.Id == change.StudentId);
            try
            {
                this.checkOnConditions(user, change);
            }
            catch (Exception exception)
            {
                return ValidationProblem();
            }

            var profile = user.Profile;

            var reduceReset = change.ReduceRest;
            var addReset = change.AddReset;
            if (reduceReset)
            {
                this.setProfileDataReduce(profile, null, null, null);
            }
            if (addReset)
            {
                this.setProfileDataAdd(profile, null, null, null);
            }
            var reducePrice = change.ReducePrice ?? 0;
            var addPrice = change.AddPrice ?? 0;
            this.userProfileRepository.updateCategoryPrice(profile, reducePrice, addPrice);
            if (change.ReduceReason != null && change.ReduceDecreeNumber != null && change.ReduceDate != null)
            {
                this.setProfileDataReduce(profile, change.ReduceReason, change.ReduceDecreeNumber, change.ReduceDate);
            }
            if (change.AddReason != null && change.AddDecreeNumber != null && change.AddDate != null)
            {
                this.setProfileDataReduce(profile, change.AddReason, change.AddDecreeNumber, change.AddDate);
            }
            this.userProfileRepository.commit();
            await this.priceRepository.setLastPrice((long)user.GroupId);
            return Ok();
        }
    }
}