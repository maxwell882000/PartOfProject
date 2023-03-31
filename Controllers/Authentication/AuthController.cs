using System;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VitcAuth.DTO.UserDTO;
using VitcAuth.Models;
using VitcAuth.Repository.Implementations;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Controllers;
using VitcAuth.Extensions;
using VitcAuth.AuthProfile.Interfaces;
using VitcAuth.AuthProfile.Exceptions;
using VitcAuth.Auth.Services;
using RestSharp;

namespace VitcAuth.Controllers
{
    [ApiController]
    [Route("apiv2/[controller]")]
    public class AuthController : ControllerBase
    {
        private UserManager<User> manager;
        private ILoginService loginService;
        private IUserRepository userRepository;
        TokenService tokenService;
        public AuthController(UserManager<User> manager,
         IUserRepository userRepository,
         ILoginService loginService,
         TokenService tokenService,
        IMapper mapper)
        {
            this.manager = manager;
            this.loginService = loginService;
            this.tokenService = tokenService;
            this.userRepository = userRepository;
        }

        [HttpPost("getToken")]
        public IActionResult getToken([FromBody] LoginDTO login)
        {
            try
            {
                var user = this.loginService.isAbleToLog(login.Username, login.Password);
                return Ok(new { userData = user, access_token = tokenService.CreateToken(user) });
            }
            catch (Exception exception)
            {
                return Unauthorized();
            }


        }
        [HttpGet("getToken/{userId}")]
        public IActionResult getTokenById(long userId)
        {
            try
            {
                return Ok(new
                {
                    token = tokenService.CreateToken(this.userRepository.GetAll().First(e => e.Id == userId))
                });
            }
            catch (Exception exception)
            {
                return Unauthorized();
            }


        }


        [HttpPost("changePassword"), Authorize]
        public IActionResult changePassword([FromBody] ChangePassword changePassword)
        {

            var user = this.manager.login(User.Claims.FirstOrDefault(e => e.Type == "UserName").Value, changePassword.old);
            if (user != null)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(changePassword.NewPassword);
                this.userRepository.update(user);
                this.userRepository.commit();
                return Ok();
            }
            return NotFound();
        }
    }
}

