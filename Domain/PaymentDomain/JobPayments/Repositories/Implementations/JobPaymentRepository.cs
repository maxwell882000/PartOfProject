using AutoMapper;
using VitcAuth.Collections.Interfaces;
using VitcAuth.DatabaseContext;
using VitcAuth.DTO.Payments;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using VitcLibrary.Repositories.Implementation;

namespace VitcAuth.Repository.Implementations
{
    public class JobPaymentRepository : GenericRepository<PostgresContext, JobStudentPayment>, IJobPaymentRepository
    {
        IPaymentCollection paymentCollection;
        IMapper mapper;
        IUserRepository userRepository;
        IBranchStudentPaymentStatusRepository branchStudentPaymentStatus;
        IHttpContextAccessor httpContextAccessor;
        public JobPaymentRepository(PostgresContext context,
                                    IPaymentCollection paymentCollection,
                                    IMapper mapper,
                                    IBranchStudentPaymentStatusRepository branchStudentPaymentStatus,
                                     IUserRepository userRepository,
                                     IHttpContextAccessor httpContextAccessor,
                                    ILogger<GenericRepository<PostgresContext, JobStudentPayment>> logger) : base(context, logger)
        {
            this.paymentCollection = paymentCollection;
            this.branchStudentPaymentStatus = branchStudentPaymentStatus;
            this.userRepository = userRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }
        public  void Remove(JobStudentPayment payment)
        {
            this.paymentCollection.DeletePayment(payment);
            this._context.JobStudentPayments.Remove(payment);
            this._context.SaveChanges();
        }
        
        public JobStudentPayment Add(CreateKasbPayment kasbPayment)
        {
            var User = this.httpContextAccessor.HttpContext.User;
            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.CreateMap<CreateKasbPayment, JobStudentPayment>()
                                .ForMember(e => e.Id, src => src.Ignore())
                                .ForMember(e => e.StudentId, src => src.MapFrom(e => e.Id))
                                .ForMember(
                                    e => e.Status, src => src.MapFrom(e =>
                                     this.branchStudentPaymentStatus.GetAll().Where(s => s.StatusName == e.Status).First().Id)
                                    )
                                .ForMember(
                                    e => e.PaidPrice, src =>
                                     src.MapFrom(e => e.PaidAmount)
                                ).ForMember(
                                    e => e.AuthorId, src => src.MapFrom(e => this.userRepository.currentUser(User).Id)
                                );
                            });
            config.CompileMappings();
            var map = config.CreateMapper();
            var payment = map.Map<JobStudentPayment>(kasbPayment);
            this._context.JobStudentPayments.Add(payment);
            this._context.SaveChanges();
            this.paymentCollection.CreateOrUpdate(payment);
            return payment;
        }
    }
}