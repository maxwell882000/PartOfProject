using VitcAuth.DatabaseContext;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Repositories.Implementation;

namespace VitcAuth.Repository.Implementations
{
    public class UserProfileRepository : GenericRepository<PostgresContext, UserProfile>, IUserProfileRepository
    {
        public UserProfileRepository(PostgresContext context, ILogger<GenericRepository<PostgresContext, UserProfile>> logger) : base(context, logger)
        {

        }

        public UserProfile calculateLostPrice(User user, DateOnly deductDate)
        {
            var group = user.Group;
            var profile = user.Profile;
            var startDate = (DateOnly)group.StartDate;
            var days = deductDate.DayNumber - startDate.DayNumber + 1;
            var wholeInterval = user.Profile.LearningDays;
            var categoryPrice = (double)profile.CategoryPrice;
            var pricePerDay = wholeInterval == 0 ? 0 : (categoryPrice - this.getSchoolPrice(user)) / wholeInterval;
            profile.LastPrice = (decimal)(days == 1 ? 0 : days * pricePerDay + this.getSchoolPrice(user));
            profile.LostPrice = (decimal)(profile.CategoryPrice - profile.LastPrice);
            profile.AbandonDays = wholeInterval - days;
            profile.AttendDays = days == 1 ? 0 : days;
            profile.TotalReduce = (decimal)profile.ReducePrice + profile.LostPrice;
            return profile;
        }

        public long getSchoolPrice(User user)
        {
            return user.CategoryId == 10 ? IUserProfileRepository.PRICE_FIRST : IUserProfileRepository.PRICE_LAST;
        }

        public async Task setLostPrice(User user, DateOnly date)
        {
            var profile = this.calculateLostPrice(user, date);
            await this.commitAsync();
        }

        public void updateCategoryPrice(UserProfile profile, long reduce, long add)
        {
            profile.CategoryPrice = (decimal?)((double)profile.CategoryPrice + (profile.ReducePrice ?? 0)
            - reduce + add - (profile.AddPrice ?? 0));
            profile.ReducePrice = reduce;
            profile.AddPrice = add;
        }
    }
}