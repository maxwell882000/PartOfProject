namespace VitcAuth.Models;

public static class JobStudentPaymentQueryExtension
{
    public static double GetRealPrice(this IQueryable<JobStudentPayment> query)
    {
        return (double)(query.Where(e => e.Status == 1).Sum(e => e.PaidPrice)
            - query.Where(e => e.Status == 2).Sum(e => e.PaidPrice));
    }
}
