using Microsoft.EntityFrameworkCore;
using VitcAuth.Models;
using VitcLibrary.Repositories.Interfaces;

namespace VitcAuth.Repository.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Group>
    {
        public IQueryable<Group> GetGroup();
    }
}