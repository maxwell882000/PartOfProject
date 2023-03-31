using AutoMapper;
using VitcAuth.Collections.Interfaces;
using VitcAuth.DatabaseContext;
using VitcAuth.DTO.Payments;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;

namespace VitcAuth.Repository.Implementations
{
    public class BranchStudentPaymentRepository : GenericRepository<PostgresContext, BranchStudentPayment>, IBranchStudentPaymentRepository
    {
        private readonly IHttpContextAccessor httpContext;
        private readonly IUserRepository userRepository;
        IDeletedPaymentRepository deletedPaymentRepository;
        IPaymentCollection paymentCollection;
        public BranchStudentPaymentRepository(
            PostgresContext context,
            IPaymentCollection paymentCollection,
            IUserRepository userRepository,
            IDeletedPaymentRepository deletedPaymentRepository,
            IHttpContextAccessor httpContext,
        ILogger<GenericRepository<PostgresContext, BranchStudentPayment>> logger) : base(context, logger)
        {
            this.paymentCollection = paymentCollection;
            this.deletedPaymentRepository = deletedPaymentRepository;
            this.userRepository = userRepository;
            this.httpContext = httpContext;
        }
        private BranchStudentPayment? validateUserPayment(long id, Func<double, User, BranchStudentPayment?> func, string status = "")
        {
            double price = 0.0;
            var user = this.userRepository.GetAll()
            .Include(e => e.Profile)
            .Include(e => e.BranchStudentPayments)
            .Where(e => e.Id == id).First();
            var User = this.httpContext?.HttpContext?.User;
            var currentUser = this.userRepository.currentUser(User);
            try
            {
                price = user.BranchStudentPayments.AsQueryable().GetRealPrice()
                 - (double)user.Profile.LastPrice;
            }
            catch (Exception exception)
            { }
            if (!user.FtestPermission && !user.UserFinalTestResults.Any()
            || user.GroupId == null || status == "incoming" || (price > 0 && status == "return")
            || currentUser.AccountantPermission)
            {
                return func(price, user);
            }
            throw new Exception("cannotremove");
        }
        public BranchStudentPayment Add(CreatePayment payment)
        {
            return this.validateUserPayment(payment.Id, (double price, User user) =>
                         {
                             var bp = this.Add(payment, user, price);
                             return bp;
                         }, payment.Status);
        }
        public void Remove(BranchStudentPayment branchStudent, DeletedPayment deletedPayment)
        {
            this.validateUserPayment(deletedPayment.StudentId, (double price, User user) =>
           {
               this.paymentCollection.DeletePayment(branchStudent);
               this.deletedPaymentRepository.Add(deletedPayment);
               this._context.BranchStudentPayments.Remove(branchStudent);
               this._context.SaveChanges();
               return null;
           });
        }

        private BranchStudentPayment Add(CreatePayment payment, User student, double price)
        {
            var User = this.httpContext?.HttpContext?.User;

            var currentUser = this.userRepository.currentUser(User);
            var config = new MapperConfiguration(cfg =>
                        {
                            cfg.CreateMap<CreatePayment, BranchStudentPayment>()
                            .ForMember(e => e.Id, src => src.Ignore())
                            .ForMember(e => e.StudentId, src => src.MapFrom(e => e.Id))
                            .ForMember(
                                e => e.Status, src => src.MapFrom(e =>
                                 this._context.BranchStudentPaymentStatuses.Where(s => s.StatusName == e.Status).First().Id)
                                )
                            .ForMember(
                                e => e.PaidPrice, src =>
                                 src.MapFrom(e => e.Status == "return"
                                 && price <= e.PaidAmount && !currentUser.AccountantPermission
                               && (student.FtestPermission || student.UserFinalTestResults.Any()) ? price
                                : e.PaidAmount)
                            ).ForMember(
                                e => e.AuthorId, src => src.MapFrom(e => currentUser.Id)
                            );
                        });
            config.CompileMappings();
            var map = config.CreateMapper();
            var bp = map.Map<BranchStudentPayment>(payment);
            this._context.BranchStudentPayments.Add(bp);
            this._context.SaveChanges();
            this.paymentCollection.CreateOrUpdate(bp);
            return bp;

        }
    }
}