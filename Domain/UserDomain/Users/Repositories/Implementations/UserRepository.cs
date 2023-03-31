using System;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Repositories.Interfaces;
using VitcLibrary.Repositories.Implementation;
using VitcAuth.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using AutoMapper;
using VitcAuth.Controllers.Moderator.Auto;
using System.Linq.Expressions;

namespace VitcAuth.Repository.Implementations
{

    public class UserRepository : GenericRepository<PostgresContext, User>, IUserRepository
    {
        private IMapper mapper;
        public UserRepository(PostgresContext context,

         IMapper mapper, ILogger<UserRepository> logger) : base(context, logger)
        {
            this.mapper = mapper;
        }
        private IQueryable<User> userWithInclude()
        {
            return _context.Users.UserWithData();
        }
        public User? FindByUserName(string name)
        {
            return this.userWithInclude()
            .FirstOrDefault(um => um.UserName == name);
        }

        public User? login(string name, string password)
        {
            User user = this.userWithInclude()
            .Include(e => e.Group)
            .Include(e => e.SystemStatusEntity)
            .FirstOrDefault(um => um.UserName == name);
            bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (verified)
            {
                return user;
            }
            return null;
        }
        public User currentOnlyUser(System.Security.Claims.ClaimsPrincipal user)
        {

            long Id = long.Parse(user.Claims.FirstOrDefault(e => e.Type == "Id").Value);
            return this._context.Users
            .First(e => e.Id == Id);
        }
        public User currentUser(System.Security.Claims.ClaimsPrincipal user)
        {

            long Id = long.Parse(user.Claims.FirstOrDefault(e => e.Type == "Id").Value);
            return this.userWithInclude()
            .First(e => e.Id == Id);
        }

        public User findByPhone(string phone)
        {
            return this._context.Users.First(e => e.PhoneNumber == phone);
        }

        private void checkPnfl(string Pnfl, long? CategoryId)
        {
            var result = this.checkUserPassport(e => e.Pinfl.Contains(Pnfl), CategoryId);
            if (result)
                throw new Exception("pinfl_exists");
        }
        private bool checkUserPassport(Expression<Func<UserPassportDatum, bool>> condition, long? CategoryId)
        {
            return this._context.UserPassportData.Where(condition).Where(e =>
             e.User.CategoryId == CategoryId
            && e.User.SystemStatus
            != this._context.SystemStatuses.Where(e => e.StatusName.Contains("deduction")).First().Id).Any();
        }
        private void checkPassport(string PassportSerial, string PassportNum, long? CategoryId)
        {
            var result = this.checkUserPassport(e => e.PassportSerial == PassportSerial
           && e.PassportNumber == PassportNum, CategoryId
           );
            if (result)
                throw new Exception("passport_exists");
        }

        public User UpdateStudent(UserEdit user)
        {
            var OldUser = this._context.Users.Include(e => e.Profile).Include(e => e.UserLicenseCategories).First(e => e.Id == user.Id);
            var NewUser = this.mapper.Map<UserEdit, User>(user, OldUser);
            NewUser.Profile = this.mapper.Map<UserEdit, UserProfile>(user, OldUser.Profile);

            this._context.UserLicenseCategories.RemoveRange(NewUser.UserLicenseCategories);
            this._context.Update(NewUser);
            this._context.SaveChanges();
            return NewUser;
        }

        public User AddStudent(UserCreate user)
        {

            this.checkPnfl(user.Passport.Pinfl, user.CategoryId);
            this.checkPassport(user.Passport.PassportNumber, user.Passport.PassportSerial, user.CategoryId);
            var create_user = this.mapper.Map<User>(user);
            create_user.UserRoles = new List<UserRole>() {new UserRole()
            {
                RoleId = user.RoleId ?? 0
            }};
            create_user.Profile = this.mapper.Map<UserProfile>(user);

            this.saveCommit(() =>
            {
                var counter = this._context.UserCounters.getStudent();
                do
                {
                    create_user.UserName = counter.Prefix + create_user.BranchId + counter.Count;
                    counter.Count++;
                    this._context.UserCounters.Update(counter);
                } while (this._context.Users.Any(e => e.UserName == create_user.UserName));
                this._context.Add(create_user);
                this._context.SaveChanges();
                return create_user;
            });

            return create_user;
        }
    }
}

