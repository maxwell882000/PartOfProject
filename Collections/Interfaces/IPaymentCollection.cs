using VitcAuth.Models;
using VitcAuth.Models.Collections;

namespace VitcAuth.Collections.Interfaces
{
    public interface IPaymentCollection
    {
        public void CreateOrUpdate(BranchStudentPayment payment);
        public void CreateOrUpdate(JobStudentPayment payment);
        public void DeletePayment(BranchStudentPayment payment);
        public void DeletePayment(JobStudentPayment payment);

    }
}