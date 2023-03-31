using System;
using VitcAuth.DTO.Payments;
using VitcAuth.Models;
using VitcLibrary.Repositories.Interfaces;

namespace VitcAuth.Repository.Interfaces
{
    public interface IJobPaymentRepository : IGenericRepository<JobStudentPayment>
    {
        public JobStudentPayment Add(CreateKasbPayment kasbPayment);
        public new void Remove(JobStudentPayment payment);
    }
}

