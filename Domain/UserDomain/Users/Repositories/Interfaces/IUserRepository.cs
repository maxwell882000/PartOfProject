using System;
using VitcAuth.Controllers.Moderator.Auto;
using VitcAuth.Models;
using VitcLibrary.Repositories.Interfaces;

namespace VitcAuth.Repository.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {

        public User? login(string name, string password);
        public User FindByUserName(string name);
        public User findByPhone(string phone);

        public User currentUser(System.Security.Claims.ClaimsPrincipal user);

        public User currentOnlyUser(System.Security.Claims.ClaimsPrincipal user);
        public User AddStudent(UserCreate user);
        public User UpdateStudent(UserEdit user);

    }
}

