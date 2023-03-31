using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VitcAuth.Collections.Interfaces;
using VitcAuth.DatabaseContext;
using VitcAuth.Models;
using VitcAuth.Models.Collections;

namespace VitcAuth.Collections
{
    public class PaymentSeed : PaymentCollection, IPaymentSeed
    {

        public PaymentSeed(IOptions<MongoDbContext> mongoDBSettings) : base(mongoDBSettings)
        {
        }
        public void Create(List<User> users)
        {
            var NOW = DateOnly.FromDateTime(DateTime.Today);

            List<PaymentAll> list = new List<PaymentAll>();
            foreach (var user in users)
            {
                var paymentAll = new PaymentAll();
                var student = user;
                var group = student?.Group;
                var branch = group?.Branch;
                var region = group?.Branch?.Region.Parent;

                paymentAll.StudentId = user.Id;
                paymentAll.RegionId = region?.Id ?? student?.Branch.Region.Parent.Id;
                paymentAll.RegionName = region?.RegionLat ?? student?.Branch?.Region?.Parent?.RegionLat;
                paymentAll.BranchId = branch?.Id ?? student?.BranchId;
                paymentAll.BranchName = branch?.BranchNameKir ?? student?.Branch?.BranchNameKir;
                paymentAll.GroupName = group?.GroupName;
                paymentAll.GroupId = group?.Id;
                paymentAll.StartDate = group?.StartDate;
                paymentAll.EndDate = group?.EndDate;

                paymentAll.IsGroupActive = !(student.GroupId == null || student?.Group?.Status == 0 || student.Group.EndDate == null);
                paymentAll.Status = group == null ?
                  (int)(StudentStatus.NO_GROUP)
                  : student?.Group?.Status == 1 ?
                  (int)StudentStatus.STUDY
                  : student?.Graduated != 1 && student?.Group?.EndDate != null && NOW >= (DateOnly)student.Group.EndDate ?
                  (int)StudentStatus.FINISHED
                   : student.Graduated == 1 ?
                  (int)StudentStatus.GRADUATED
                   : (int)StudentStatus.REST;

                paymentAll.IsRemoved = student.SystemStatus == 1;
                paymentAll.DeductDate = student?.SystemStatusSteps?.OrderByDescending(e => e.Id).FirstOrDefault()?.DeductDate;


                paymentAll.Payment = user.BranchStudentPayments.ToList().OrderBy(e => e.Id).Select(payment =>
                {
                    var paid = (double)(payment.Status == 1 ? payment?.PaidPrice ?? 0 : -1 * payment?.PaidPrice ?? 0);
                    return new Payment()
                    {
                        Id = payment.Id,
                        PaidDate = (DateOnly)payment.PaidDate,
                        PaidPrice = paid,
                    };
                }).ToList<Payment>();
                paymentAll.IsKasb = 0;

                list.Add(paymentAll);
            }
            this._payment.InsertMany(list);
        }



        public void Create(List<StudentJob> users)
        {
            var NOW = DateOnly.FromDateTime(DateTime.Today);

            List<PaymentAll> list = new List<PaymentAll>();
            foreach (var user in users)
            {
                var paymentAll = new PaymentAll();
                var student = user;
                var group = student?.Group;
                var branch = group?.Branch;
                var region = group?.Branch?.Region.Parent;

                paymentAll.StudentId = user.Id;
                paymentAll.RegionId = region?.Id ?? student?.Branch.Region.Parent.Id;
                paymentAll.RegionName = region?.RegionLat ?? student?.Branch?.Region?.Parent?.RegionLat;

                paymentAll.BranchId = branch?.Id ?? student?.BranchId;
                paymentAll.BranchName = branch?.BranchNameKir ?? student?.Branch?.BranchNameKir;
                paymentAll.GroupName = group?.GroupName;
                paymentAll.GroupId = group?.Id;
                paymentAll.StartDate = group?.StartDate;
                paymentAll.EndDate = group?.EndDate;

                paymentAll.IsGroupActive = group?.StartDate < NOW || group?.StartDate == null;
                paymentAll.Status = group == null ?
                            (int)(StudentStatus.NO_GROUP) :
                            student?.Group?.StartDate <= NOW && NOW >= student?.Group?.EndDate ?
                            (int)StudentStatus.STUDY :
                            NOW > (DateOnly)student.Group.EndDate ?
                            (int)StudentStatus.GRADUATED :
                            (int)StudentStatus.REST;

                paymentAll.IsRemoved = student.SystemStatus == 1;
                paymentAll.DeductDate = student?.SystemStatusHistories?.OrderByDescending(e => e.Id).FirstOrDefault()?.Date;
                paymentAll.Payment = student.Payment.OrderBy(e => e.Id).Select(payment =>
                {
                    var paid = (double)(payment.Status == 1 ? payment?.PaidPrice ?? 0 : -1 * payment?.PaidPrice ?? 0);
                    return new Payment()
                    {
                        Id = payment.Id,
                        PaidDate = (DateOnly)payment.PaidDate,
                        PaidPrice = paid,
                    };
                }).ToList<Payment>();
                paymentAll.IsKasb = 1;
                list.Add(paymentAll);
            }
            this._payment.InsertMany(list);
        }
    }
}