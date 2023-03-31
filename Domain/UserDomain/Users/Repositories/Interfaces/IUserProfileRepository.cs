using VitcAuth.Models;
using VitcLibrary.Repositories.Interfaces;

namespace VitcAuth.Repository.Interfaces
{
    public interface IUserProfileRepository : IGenericRepository<UserProfile>
    {
        public const long PRICE_LAST = 50000;
        public const long PRICE_FIRST = 15000;
        public UserProfile calculateLostPrice(User user, DateOnly date);
        public Task setLostPrice(User user, DateOnly date);
        public long getSchoolPrice(User user);
        public void updateCategoryPrice(UserProfile profile, long reduce, long add);
    }
}