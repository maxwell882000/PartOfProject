using LinqKit;
using VitcAuth.Controllers.Moderator.Auto;
using VitcAuth.DatabaseContext;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Repositories.Implementation;

namespace VitcAuth.Repository.Implementations
{
    public static class StudentJobExtension
    {
        public static IQueryable<StudentJob> Filter(
            this IQueryable<StudentJob> query,
             string? search = null,
             long? SystemStatus = null,
             long? GroupId = null,
             bool IsNullGroup = false
            )
        {

            var predicate = PredicateBuilder.New<StudentJob>();
            if (search != null)
                predicate.And(e =>
                e.Firstname.ToLower().Contains(search) ||
                e.Lastname.ToLower().Contains(search)
                );
            if (IsNullGroup)
                predicate.And(e => e.GroupId == null);
            if (SystemStatus != null)
                predicate.And(e => e.SystemStatus == SystemStatus);
            if (GroupId != null)
                predicate.And(e => e.GroupId == GroupId);
            return query.Where(predicate);
        }
    }
    public class StudentJobRepository : GenericRepository<PostgresContext, StudentJob>, IStudentJobRepository
    {
        public StudentJobRepository(PostgresContext context, ILogger<GenericRepository<PostgresContext, StudentJob>> logger) : base(context, logger)
        {
        }
        public StudentJob UpdateStudent(StudentJob user)
        {
            this._context.Update(user);
            this._context.SaveChanges();
            return user;
        }
        public StudentJob AddStudent(StudentJob studentJob)
        {
            var Branch = this._context.Branches.First(e => e.Id == studentJob.BranchId);
            studentJob.CompanyId = Branch.CompanyId;
            studentJob.OrganizationId = Branch.OrganizationId;
            var counter = this._context.UserCounters.getProfession();
            do
            {
                studentJob.Username = counter.Prefix + counter.Count;
                counter.Count++;
                this._context.UserCounters.Update(counter);
            } while (this._context.StudentJobs.Any(e => e.Username == studentJob.Username));
            this._context.StudentJobs.Add(studentJob);
            this._context.SaveChanges();
            return studentJob;
        }

    }
}