using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcAuth.Repository.Implementations;

using VitcLibrary.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using VitcAuth.DTO.UserDTO;

namespace VitcAuth.Controllers.Users
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class UserController : GenericController<IUserRepository, User, User>
    {
        IOwnerSecretRepository ownerSecret;
        public UserController(IUserRepository userRepository, IOwnerSecretRepository ownerSecret, IMapper mapper) : base(userRepository, mapper)
        {
            this.ownerSecret = ownerSecret;
        }

        [HttpGet("{Id}")]
        public override IActionResult Get(long Id)
        {
            var user = this._repository.GetAll().UserWithData()
            .FirstOrDefault(e => e.Id == Id);
            return Ok(user);
        }

        [HttpDelete("{Id}")]
        public IActionResult Delete(long UserId)
        {
            var User = this._repository.GetById(UserId);
            if (User.GroupId == null)
            {
                this._repository.Remove(User);
                this._repository.commit();
            }
            return Ok();
        }
        [HttpGet("setLanguage")]
        public IActionResult setLanguage(string lang)
        {
            var user = this._repository.currentUser(User);
            user.Lang = lang;
            this._repository.update(user);
            this._repository.commit();
            return Ok(user);
        }



        [HttpGet("UserData")]
        public IActionResult UserData()
        {
            var rqf = HttpContext.Features.Get<RequestLocalizationOptions>();
            var current = this._repository.currentUser(User);
            var user = this._mapper.Map<MainUser>(current);
            return Ok(user);
        }

        [HttpGet("GroupSecrets/{GroupId}")]
        public IActionResult GroupSecrets(long GroupId)
        {
            var students = this._repository.GetAll()
            .Include(e => e.UserSecret)
            .Include(e => e.Profile)
            .Where(e => e.GroupId == GroupId).ToList();
            return Ok(new { data = students });
        }

        [HttpGet("UserSecrets/{UserId}")]
        public IActionResult UserSecrets(long UserId)
        {
            var secret = this.ownerSecret.GetAll()

            .Where(e => e.OwnerId == UserId).First();
            return Ok(new { data = secret.Secret });
        }
    }
}