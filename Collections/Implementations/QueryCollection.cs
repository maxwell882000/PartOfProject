
using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VitcStatApi.Collections.Interfaces;
using VitcStatApi.DatabaseContext;
using VitcStatApi.Models;
using LinqKit;
namespace VitcStatApi.Collections
{
    public class PaymentCollection : IPaymentCollection
    {
        private readonly IMongoCollection<PaymentAll> _payment;
        private readonly ILogger<PaymentCollection> _logger;
        public PaymentCollection(IOptions<MongoDbContext> mongoDBSettings, ILogger<PaymentCollection> logger)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionURI);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            this._logger = logger;
            _payment = database.GetCollection<PaymentAll>(mongoDBSettings.Value.CollectionName);
        }

        private ProjectionDefinition<PaymentAll, PaymentProjection> GetProjection(DateOnly? fromDate = null,
        DateOnly? toDate = null)
        {
        
            return Builders<PaymentAll>.Projection
                        .Expression(e => new PaymentProjection()
                        {
                            Model = e,
                            CumulativePrice = e.Payment.Where(e => (e.PaidDate >= fromDate || fromDate == null) &&
                            (e.PaidDate <= toDate || toDate == null)
                            ).Sum(e => e.PaidPrice),
                            PaidNumber = e.Payment.Where(e => fromDate != null
                            ? e.PaidDate >= fromDate : true && toDate != null ? e.PaidDate <= toDate : true
                            ).OrderBy(e => e.Id).Count()
                        });
        }
        private FilterDefinition<PaymentAll> getMatch(
            DateOnly? fromDatePaid,
        DateOnly? toDatePaid, long? RegionId = null,
         long? BranchId = null, int? isKasb = null)
        {
            var builder = Builders<Payment>.Filter;
            var paymentBuilder = Builders<PaymentAll>.Filter;
            FilterDefinition<Payment> filterPayment = builder.Empty;
            FilterDefinition<PaymentAll> filter = paymentBuilder.Empty;
            if (fromDatePaid != null)
            {
                filterPayment &= builder.Gte(e => e.PaidDate, (DateOnly)fromDatePaid);
            }

            if (toDatePaid != null)
            {
                filterPayment &= builder.Lte(e => e.PaidDate, (DateOnly)toDatePaid);
            }
            filter &= paymentBuilder.ElemMatch(e => e.Payment, filterPayment);
            if (RegionId != null)
            {
                filter &= paymentBuilder.Where(e => e.RegionId == RegionId);
            }
            if (BranchId != null)
            {
                filter &= paymentBuilder.Where(e => e.BranchId == BranchId);
            }
            if (isKasb != null)
            {
                filter &= paymentBuilder.Where(e => e.IsKasb == isKasb);
            }

            return filter;
        }

        public List<RegionPayment> getRegionResult(DateOnly? fromDatePaid, DateOnly? toDatePaid, int? isKasb = null,
         bool isNotGroup = false)
        {

            List<RegionPayment> payment = _payment.Aggregate()
                            .Match(getMatch(fromDatePaid, toDatePaid, isKasb: isKasb))
                            // .Match(e => (e.GroupId == null && isNotGroup) || (e.GroupId != null && !isNotGroup))
                            .Project(
                                GetProjection(fromDatePaid, toDatePaid)
                            )
                            // .Match(e => e.Model.IsKasb == 1)
                            .Group(e => new { e.Model.RegionId, e.Model.RegionName },
                             g => new RegionPayment()
                             {
                                 RegionId = g.Key.RegionId,
                                 PaidStudentNumber = g.Count(),
                                 PaidNumber = g.Sum(g => g.PaidNumber),
                                 RegionName = g.Key.RegionName,
                                 CumulativePrice = g.Sum(e => e.CumulativePrice)
                             }
                             ).ToList<RegionPayment>();
            return payment;
        }

        public List<BranchPayment> getBranchResult(long? RegionId, DateOnly? fromDatePaid, DateOnly? toDatePaid, int? isKasb = null,
         bool isNotGroup = false)
        {
            List<BranchPayment> payment = _payment.Aggregate()
                             .Match(getMatch(fromDatePaid, toDatePaid, RegionId, isKasb: isKasb))
                            //  .Match(e => (e.GroupId == null && isNotGroup) || (e.GroupId != null && !isNotGroup))
                            .Project(
                                GetProjection(fromDatePaid, toDatePaid)
                            )
                            .Group(e => new
                            {
                                e.Model.RegionId,
                                e.Model.RegionName,
                                e.Model.BranchId,
                                e.Model.BranchName
                            },
                             g => new BranchPayment
                             {
                                 RegionId = g.Key.RegionId,
                                 PaidStudentNumber = g.Count(),
                                 BranchId = g.Key.BranchId,
                                 BranchName = g.Key.BranchName,
                                 PaidNumber = g.Sum(g => g.PaidNumber),
                                 RegionName = g.Key.RegionName,
                                 CumulativePrice = g.Sum(e => e.CumulativePrice)
                             }).ToList<BranchPayment>();
            return payment;
        }

        public List<GroupPayment> getGroupResult(long? BranchId,
         DateOnly? fromDatePaid,
         DateOnly? toDatePaid,
         bool isNotGroup = false,
         int? isKasb = null
         )
        {
            var payment = _payment.Aggregate()
                            .Match(getMatch(fromDatePaid, toDatePaid, BranchId: BranchId, isKasb: isKasb))
                            .Match(e => (e.GroupId == null && isNotGroup) || (e.GroupId != null && !isNotGroup))
                            .Project(
                                GetProjection(fromDatePaid, toDatePaid)
                            )
                            .Group(e => new
                            {
                                e.Model.RegionId,
                                e.Model.RegionName,
                                e.Model.BranchId,
                                e.Model.BranchName,
                                e.Model.GroupName,
                                e.Model.GroupId
                            },
                             g => new GroupPayment
                             {
                                 RegionId = g.Key.RegionId,
                                 PaidStudentNumber = g.Count(),
                                 BranchId = g.Key.BranchId,
                                 BranchName = g.Key.BranchName,
                                 GroupName = g.Key.GroupName,
                                 GroupId = g.Key.GroupId,
                                 PaidNumber = g.Sum(g => g.PaidNumber),
                                 RegionName = g.Key.RegionName,
                                 CumulativePrice = g.Sum(e => e.CumulativePrice)
                             });
            this._logger.LogError(payment.ToString());
            return payment.ToList<GroupPayment>();
        }
    }
}