using VitcAuth.Models;
using Microsoft.EntityFrameworkCore;
using LinqKit;

namespace VitcAuth.Repository.Implementations
{
    public static class UserFilterExtension
    {

        public static IQueryable<User> UserWithData(this IQueryable<User> query)
        {
            return query
            .Include(e => e.Branch)
            .Include(e => e.Profile)
            .Include(e => e.Region)
            .Include(e => e.Role)
            .Include(e => e.Company)
            .Include(e => e.Organization)
            .Include(e => e.Contacts)
            .Include(e => e.Passport)
            .Include(e => e.Roles)
            .AsSplitQuery();
        }
        public static IQueryable<User> ListStudent(this IQueryable<User> query)
        {
            return query
            .Include(e => e.Profile)
            .Include(e => e.Passport)
            .Include(e => e.Category)
            .Include(e => e.Group)
            .Where(e => e.RoleId == 7);
        }

        public static IQueryable<User> GetFiredStudent(this IQueryable<User> query)
        {
            return query
             .Where(e => e.RoleId == 7
            && e.SystemStatus == 1)
            .Include(e => e.Group)
            .Include(e => e.Category)
            .Include(e => e.SystemStatusSteps)
            .ThenInclude(e => e.Status)
            .Include(e => e.SystemStatusSteps.OrderByDescending(e => e.DeductDate))
            .ThenInclude(e => e.Reason)
            .OrderByDescending(e => e.SystemStatusSteps.First().DeductDate);
        }

        public static IQueryable<User> ShowStudent(this IQueryable<User> query)
        {
            return query.ListStudent()
            .Include(e => e.Roles)
            .Include(e => e.EducationLevel)
            .Include(e => e.UserLicenseCategories)
            .Include(e => e.UserLicenses)
            .Include(e => e.Contacts)
            .Include(e => e.StudentCertificate)
            .Include(e => e.Role);
        }

        public static IQueryable<User> Filter(this IQueryable<User> query,
        string? search = null,
        long? GroupId = null,
        bool? IsPreferential = null,
        long? SystemStatusNot = null
        )
        {
            var predicate = PredicateBuilder.New<User>();
            if (search != null)
            {
                predicate.And(e =>
                 e.Profile.Firstname.ToLower().Contains(search.ToLower())
                || e.Profile.Lastname.ToLower().Contains(search.ToLower())
                || e.Passport.PassportNumber.ToLower().Contains(search.ToLower())
                || e.Passport.PassportSerial.ToLower().Contains(search.ToLower())
                || (e.Passport.PassportSerial + e.Passport.PassportNumber).ToLower().Contains(search.ToLower())
                || e.UserName.ToLower().Contains(search));
            }
            if (SystemStatusNot != null)
                predicate.And(e => e.SystemStatus != SystemStatusNot);
            if (GroupId != null)
                predicate.And(e => e.GroupId == GroupId);

            if (IsPreferential != null)
                predicate.And(e => e.IsPreferential == IsPreferential);

            return query.Where(predicate);
        }
    }
}

