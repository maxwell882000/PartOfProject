using VitcAuth.DatabaseContext;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Repositories.Implementation;

namespace VitcAuth.Repository.Implementations
{
    public class ProfessionGroupRepository : GenericRepository<PostgresContext, ProfessionGroup>, IProfessionGroupRepository
    {
        public ProfessionGroupRepository(PostgresContext context, ILogger<ProfessionGroupRepository> logger) : base(context, logger)
        {

        }
    }
}

