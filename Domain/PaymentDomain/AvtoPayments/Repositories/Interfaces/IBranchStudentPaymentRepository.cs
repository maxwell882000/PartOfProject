using System;
using VitcAuth.DTO.Payments;
using VitcAuth.Models;
using VitcLibrary.Repositories.Interfaces;

namespace VitcAuth.Repository.Interfaces
{
    public interface IBranchStudentPaymentRepository : IGenericRepository<BranchStudentPayment>
    {
        public BranchStudentPayment Add(CreatePayment payment);
        public void Remove(BranchStudentPayment branchStudent, DeletedPayment deletedPayment);
    }
}

