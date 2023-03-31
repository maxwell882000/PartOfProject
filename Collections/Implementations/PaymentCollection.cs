using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VitcAuth.Collections.Interfaces;
using VitcAuth.DatabaseContext;
using VitcAuth.Models;
using VitcAuth.Models.Collections;

namespace VitcAuth.Collections
{
    public class PaymentCollection : IPaymentCollection
    {
        protected readonly IMongoCollection<PaymentAll> _payment;

        public PaymentCollection(IOptions<MongoDbContext> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _payment = database.GetCollection<PaymentAll>(mongoDBSettings.Value.CollectionName);
        }



        public void CreateOrUpdate(BranchStudentPayment payment)
        {
            var builder = Builders<PaymentAll>.Filter;
            var filter = builder.And(builder.Eq("IsKasb", 0) & builder.Eq("StudentId", payment.StudentId));
            var query = _payment.Find(e => e.IsKasb == 0 && e.StudentId == payment.StudentId).FirstOrDefault();
            var PaidPrice = (double)(payment.Status == 1 ? payment?.PaidPrice ?? 0 : -1 * payment?.PaidPrice ?? 0);
            payment.getRelated("Student");
            payment?.Student.getRelated("Group");
            if (query != null)
            {
                if (!query.Payment.Exists(e => e.Id == payment.Id))
                {
                    var updateBuilder = Builders<PaymentAll>.Update;
                    var update = updateBuilder.Push(e => e.Payment, new Payment()
                    {
                        Id = payment.Id,
                        PaidDate = (DateOnly)payment.PaidDate,
                        PaidPrice = PaidPrice,
                    }).Set(e => e.GroupId, payment.Student.GroupId)
                    .Set(e => e.GroupName, payment?.Student?.Group?.GroupName)
                    .Set(e => e.IsGroupActive, !(payment?.Student.GroupId == null || payment?.Student?.Group?.Status == 0 || payment?.Student?.Group?.EndDate == null));

                    _payment.UpdateOne(filter, update);
                }
            }
            else
            {
                var NOW = DateOnly.FromDateTime(DateTime.Today);

                var student = payment?.Student;
                var group = student?.Group;
                group?.getRelated("Branch");
                var branch = group?.Branch;
                branch?.getRelated("Region");
                var region = branch?.Region;
                region?.getRelated("Parent");
                payment?.Student.getRelated("Region");
                payment?.Student.getRelated("Branch");
                payment?.Student.getRelated("SystemStatusSteps");
                payment?.Student.Region.getRelated("Parent");

                payment?.Student.getRelated("Branch");
                var BranchStudent = payment?.Student?.Branch;
                BranchStudent?.getRelated("Region");
                var BranchRegion = BranchStudent?.Region;
                BranchRegion?.getRelated("Parent");
                var paymentAll = new PaymentAll();
                paymentAll.RegionName = region?.Parent?.RegionLat ?? BranchRegion?.Parent.RegionLat;

                paymentAll.StudentId = payment.StudentId;
                paymentAll.RegionId = region?.ParentId ?? BranchRegion?.Parent.Id;
                paymentAll.BranchId = branch?.Id ?? student?.BranchId;
                paymentAll.BranchName = branch?.BranchNameKir ?? payment?.Student?.Branch?.BranchNameKir;
                paymentAll.GroupName = group?.GroupName;
                paymentAll.GroupId = group?.Id;
                paymentAll.StartDate = group?.StartDate;
                paymentAll.EndDate = group?.EndDate;

                paymentAll.IsGroupActive = !(payment?.Student.GroupId == null || payment?.Student?.Group?.Status == 0 || student.Group.EndDate == null);
                paymentAll.Status = group == null ?
                  (int)(StudentStatus.NO_GROUP)
                  : payment?.Student?.Group?.Status == 1 ?
                  (int)StudentStatus.STUDY
                  : payment?.Student?.Graduated != 1 && student.Group.EndDate != null && NOW >= (DateOnly)payment.Student.Group.EndDate ?
                  (int)StudentStatus.FINISHED
                   : payment.Student.Graduated == 1 ?
                  (int)StudentStatus.GRADUATED
                   : (int)StudentStatus.REST;
                paymentAll.IsRemoved = payment?.Student.SystemStatus == 1;
                paymentAll.DeductDate = payment?.Student?.SystemStatusSteps?.OrderByDescending(e => e.Id).FirstOrDefault()?.DeductDate;
                paymentAll.Payment = new List<Payment>() { new Payment()
                    {
                        Id = payment.Id,
                        PaidDate = (DateOnly)payment.PaidDate,
                        PaidPrice = PaidPrice,
                    }
                };
                paymentAll.IsKasb = 0;

                this._payment.InsertOne(paymentAll);
            }
        }


        public void CreateOrUpdate(JobStudentPayment payment)
        {

            var builder = Builders<PaymentAll>.Filter;
            var filter = builder.And(builder.Eq("IsKasb", 1) & builder.Eq("StudentId", payment.StudentId));
            var query = _payment.Find(e => e.IsKasb == 1 && e.StudentId == payment.StudentId).FirstOrDefault();
            var PaidPrice = (double)(payment.Status == 1 ? payment?.PaidPrice ?? 0 : -1 * payment?.PaidPrice ?? 0);

            payment.getRelated("Student");
            payment?.Student.getRelated("Group");
            if (query != null)
            {
                if (!query.Payment.Exists(e => e.Id == payment.Id))
                {
                    var update = Builders<PaymentAll>.Update.Push(e => e.Payment, new Payment()
                    {
                        Id = payment.Id,
                        PaidDate = (DateOnly)payment.PaidDate,
                        PaidPrice = PaidPrice,
                    }).Set(e => e.GroupId, payment.Student.GroupId)
                    .Set(e => e.GroupName, payment?.Student?.Group?.GroupName)
                    .Set(e => e.IsGroupActive, !(payment?.Student.GroupId == null || payment?.Student?.Group?.Status == 0 || payment?.Student?.Group?.EndDate == null));
                    
                    _payment.UpdateOne(filter, update);
                }
            }
            else
            {
                var NOW = DateOnly.FromDateTime(DateTime.Today);
                payment.getRelated("Student");
                payment?.Student.getRelated("Group");
                var student = payment?.Student;
                var group = student?.Group;
                group?.getRelated("Branch");
                var branch = group?.Branch;
                branch?.getRelated("Region");
                var region = branch?.Region;
                region?.getRelated("Parent");
                payment?.Student.getRelated("Region");
                payment?.Student.getRelated("Branch");
                payment?.Student.getRelated("SystemStatusHistories");
                payment?.Student.Region.getRelated("Parent");

                payment?.Student.getRelated("Branch");
                var BranchStudent = payment?.Student?.Branch;
                BranchStudent?.getRelated("Region");
                var BranchRegion = BranchStudent?.Region;
                BranchRegion?.getRelated("Parent");
                var paymentAll = new PaymentAll();
                paymentAll.RegionName = region?.Parent?.RegionLat ?? BranchRegion?.Parent.RegionLat;
                paymentAll.StudentId = payment.StudentId;
                paymentAll.RegionId = region?.ParentId ?? BranchRegion?.Parent.Id;
                paymentAll.BranchId = branch?.Id ?? student?.BranchId;
                paymentAll.BranchName = branch?.BranchNameKir ?? payment?.Student?.Branch?.BranchNameKir;
                paymentAll.GroupName = group?.GroupName;
                paymentAll.GroupId = group?.Id;
                paymentAll.StartDate = group?.StartDate;
                paymentAll.EndDate = group?.EndDate;
                paymentAll.IsGroupActive = group?.StartDate < NOW || group?.StartDate == null;
                paymentAll.Status = group == null ?
                  (int)(StudentStatus.NO_GROUP) :
                  payment?.Student?.Group?.StartDate <= NOW && NOW >= payment?.Student?.Group?.EndDate ?
                  (int)StudentStatus.STUDY :
                   NOW > (DateOnly)payment.Student.Group.EndDate ?
                  (int)StudentStatus.GRADUATED :
                  (int)StudentStatus.REST;
                paymentAll.IsRemoved = payment?.Student.SystemStatus == 1;
                paymentAll.DeductDate = student?.SystemStatusHistories?.OrderByDescending(e => e.Id).FirstOrDefault()?.Date;
                paymentAll.Payment = new List<Payment>() { new Payment()
             {
                        Id = payment.Id,
                        PaidDate = (DateOnly)payment.PaidDate,
                        PaidPrice = (double)PaidPrice,
                    }
                 };
                paymentAll.IsKasb = 1;

                _payment.InsertOne(paymentAll);
            }
        }

        public void DeletePayment(BranchStudentPayment payment)
        {
            var builder = Builders<PaymentAll>.Filter;
            FilterDefinition<PaymentAll> filter = builder.And(builder.Eq("IsKasb", 0) & builder.Eq("StudentId", payment.StudentId));
            var update = Builders<PaymentAll>.Update
            .PullFilter(e => e.Payment, x => x.Id == payment.Id);
            _payment.FindOneAndUpdate(
               filter,
               update);
        }


        public void DeletePayment(JobStudentPayment payment)
        {

            var builder = Builders<PaymentAll>.Filter;
            FilterDefinition<PaymentAll> filter = builder.And(builder.Eq("IsKasb", 1) & builder.Eq("StudentId", payment.StudentId));
            var update = Builders<PaymentAll>.Update
            .PullFilter(e => e.Payment, x => x.Id == payment.Id);
            _payment.FindOneAndUpdate(
               filter,
               update
            );
        }


    }
}