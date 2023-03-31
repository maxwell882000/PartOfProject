using VitcAuth.Models;

namespace VitcAuth.Models
{
    public static class BranchStudentQueryExtension
    {
        public static double GetRealPrice(this IQueryable<BranchStudentPayment> query)
        {
            return (query.Where(e => e.Status == 1).Sum(e => e.PaidPrice)
                - query.Where(e => e.Status == 2).Sum(e => e.PaidPrice)) ?? 0;
        }
    }
}

