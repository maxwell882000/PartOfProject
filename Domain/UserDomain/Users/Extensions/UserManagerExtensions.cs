using Microsoft.AspNetCore.Identity;
using VitcAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace VitcAuth.Extensions
{
    public static class UserManagerExtensions
    {
        public static User FindByUserName(this UserManager<User> userManager, string name)
        {
            return userManager?.Users?
            .Include(e => e.Branch)
            .Include(e => e.Profile)
            .Include(e => e.Region)
            .Include(e => e.Role)
            .Include(e => e.Company)
            .Include(e => e.Organization)
            .Include(e => e.Contacts)
            .Include(e => e.Passport)
            .Include(e => e.Roles)
            .FirstOrDefault(um => um.UserName == name);
        }

        public static User? login(this UserManager<User> userManager, string name, string password)
        {
            User user = userManager?.Users
            .Include(e => e.Branch)
            .Include(e => e.Profile)
            .Include(e => e.Region)
            .Include(e => e.Role)
            .Include(e => e.Company)
            .Include(e => e.Organization)
            .Include(e => e.Contacts)
            .Include(e => e.Group)
            .Include(e => e.SystemStatusEntity)
            .Include(e => e.Passport)
            .Include(e => e.Roles)
            .AsSplitQuery()
            .FirstOrDefault(um => um.UserName == name);
            bool verified = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (verified)
            {
                return user;
            }
            return null;
        }
    }
}

