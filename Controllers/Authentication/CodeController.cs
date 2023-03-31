using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcAuth.Services.PhoneService.Interfaces;

namespace VitcAuth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CodeController : ControllerBase
    {

        private IPhoneService phoneService;

        private IUserRepository repository;

        private UserManager<User> userManager;
        public CodeController(IPhoneService phoneService, IUserRepository repository, UserManager<User> userManager)
        {
            this.phoneService = phoneService;
            this.repository = repository;
            this.userManager = userManager;
        }


        [HttpGet("verifyCode")]
        public async Task<ActionResult> verifyCode(int code)
        {
            var user = await this.userManager.GetUserAsync(User);
            if (user.verificationCode == code)
            {
                user.PhoneNumberConfirmed = true;
                this.repository.commit();
                return Ok();
            }
            return Forbid();
        }

        [HttpGet("sendCode")]
        public IActionResult sendCode(string phone)
        {
            try
            {
                var random = new Random();
                User user = this.repository.findByPhone(phone);
                user.verificationCode = random.Next(100000, 999999);
                this.repository.commit();
                this.phoneService.sendCode(phone, "Ваш код для регистрации : " + user.verificationCode);
                return Ok();
            }
            catch (Exception exception)
            {
                return NotFound(exception.Message);
            }
        }
    }
}

