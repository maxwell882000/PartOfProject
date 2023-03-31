using VitcAuth.Collections.Interfaces;
using VitcAuth.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using VitcLibrary.Helpers;
using VitcAuth.Models;

namespace VitcAuth.Seeds
{
    public class MainSeed : IMainSeed
    {
        private IPaymentSeed paymentCollection;
        private IUserRepository userRepository;
        private IStudentJobRepository studentJobRepository;
        ILogger<MainSeed> logger;
        public MainSeed(IPaymentSeed paymentCollection,
        IJobPaymentRepository jobPaymentRepository,
        IUserRepository userRepository,
        IStudentJobRepository studentJobRepository,
        IBranchStudentPaymentRepository branchStudentPaymentRepository,
        ILogger<MainSeed> logger
        )
        {
            this.userRepository = userRepository;
            this.studentJobRepository = studentJobRepository;
            this.paymentCollection = paymentCollection;
            this.logger = logger;
        }
        // Now we can use projection so we can find students overall and who paid  making new filter and then count
        // so we will during projection assign values who paid and who is not paid 
        // and then count them using count function 
        public void seed()
        {
            logger.LogError("SEED STARTED ======");
            List<long> organizations = new List<long> { 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
            logger.LogCritical(this.studentJobRepository.GetAll().Count().ToString());
            var counter = 0;
            var page = 1;
            while (true)
            {
                logger.LogCritical(page.ToString());
                var pagination = new Pagination<StudentJob>(
                    studentJobRepository.GetAll()
                .Include(e => e.Payment)
                .Include(e => e.SystemStatusHistories)
                .Include(e => e.Group)
                .ThenInclude(e => e.Branch)
                .ThenInclude(e => e.Region)
                .ThenInclude(e => e.Parent)
                .Include(e => e.Branch)
                .ThenInclude(e => e.Region)
                .ThenInclude(e => e.Parent)
                .AsSplitQuery()
                .Where(e => organizations.Contains(e.OrganizationId ?? 0)), page, 100000);
                (IQueryable<StudentJob> pagintated, int totalPage) = pagination.paginate();
                var result = pagintated.ToList();
                this.paymentCollection.Create(result);
                if (page == totalPage)
                    break;
                else
                    page += 1;
            }
            page = 1;
            while (true)
            {
                logger.LogCritical(page.ToString());
                var pagination = new Pagination<User>(
                   userRepository.GetAll()
                        .Include(e => e.SystemStatusSteps)
                        .Include(e => e.BranchStudentPayments)
                        .Include(e => e.Group)
                        .ThenInclude(e => e.Organization)
                        .ThenInclude(e => e.Region)
                        .Include(e => e.Group)
                        .ThenInclude(e => e.Branch)
                        .ThenInclude(e => e.Region)
                        .ThenInclude(e => e.Parent)
                        .Include(e => e.Branch)
                        .ThenInclude(e => e.Region)
                        .ThenInclude(e => e.Parent)
                        .AsSplitQuery()
                        .Where(e => organizations.Contains(e.OrganizationId ?? 0) && e.RoleId == 7 && e.Preferential != 6), page, 100000);
                (IQueryable<User> pagintated, int totalPage) = pagination.paginate();
                var result = pagintated.ToList();
                this.paymentCollection.Create(result);
                if (page == totalPage)
                    break;
                else
                    page += 1;
            }
            logger.LogCritical("SEED ENDED ====== " + counter.ToString());
        }
    }
}