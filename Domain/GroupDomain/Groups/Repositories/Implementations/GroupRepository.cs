using System.Linq.Expressions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using VitcAuth.DatabaseContext;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Repositories.Implementation;

namespace VitcAuth.Repository.Implementations
{
    public static class GroupFilterExtension
    {
        public static IQueryable<Group> Filter(
            this DbSet<Group> query,
            long? CategoryId = null,
        long? BranchId = null,
        int? Status = null,
        int? StatusNot = null)
        {
            var predicate = PredicateBuilder.New<Group>();

            if (CategoryId != null)
                predicate.And(e => e.CategoryId == CategoryId);
            if (BranchId != null)
                predicate.And(e => e.BranchId == BranchId);
            if (Status != null)
                predicate.And(e => e.Status == Status);
            if (StatusNot != null)
                predicate.And(e => e.Status != StatusNot);
            return query.Where(predicate);

        }
    }

    public class GroupRepository : GenericRepository<PostgresContext, Group>, IGroupRepository
    {
        public GroupRepository(PostgresContext context, ILogger<GenericRepository<PostgresContext, Group>> logger) : base(context, logger)
        {
        }

        public IQueryable<Group> GetGroup()
        {

            return this._context.Groups
            .Include(e => e.Category)
            .Include(e => e.Period)
            .Include(e => e.MainTeacherEntity)
            .ThenInclude(e => e.Profile)
            .Include(e => e.MainAssistantTeacherEntity)
            .Include(e => e.MedTeacherEntity)
            .ThenInclude(e => e.Profile)
            .Include(e => e.PracticeTeachers)
            .ThenInclude(e => e.Profile);
        }
    }
}