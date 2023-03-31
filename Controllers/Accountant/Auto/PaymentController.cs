using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VitcAuth.DTO.Payments;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using VitcAuth.Collections.Interfaces;

namespace VitcAuth.Controllers.Accountant
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class PaymentController : ControllerBase
    {
        private IBranchStudentPaymentRepository branchStudentPayment;
        private IBranchStudentPaymentStatusRepository branchStudentPaymentStatus;

        private IUserRepository userRepository;

        private IMapper mapper;

        private IPaymentCollection paymentCollection;

        private IViewsRepository views;

        private IDeletedPaymentRepository deletedPaymentRepository;
        public PaymentController(IBranchStudentPaymentRepository branchStudentPayment,
         IBranchStudentPaymentStatusRepository branchStudentPaymentStatus,
         IUserRepository userRepository,
         IPaymentCollection paymentCollection,
         IDeletedPaymentRepository deletedPaymentRepository,
         IViewsRepository views,
         IMapper mapper
         )
        {
            this.branchStudentPayment = branchStudentPayment;
            this.branchStudentPaymentStatus = branchStudentPaymentStatus;
            this.userRepository = userRepository;
            this.paymentCollection = paymentCollection;
            this.deletedPaymentRepository = deletedPaymentRepository;
            this.views = views;
            this.mapper = mapper;
        }

        [HttpPost("makePayment")]
        public IActionResult makePayment(CreatePayment payment)
        {
            try
            {
                return Ok(this.mapper.Map<StudentPayment>(this.branchStudentPayment.Add(payment)));

            }
            catch (Exception exception)
            {
                ModelState.AddModelError("data", exception.Message);
                return ValidationProblem();
            }
        }

        [HttpDelete("removePayment")]
        public IActionResult removePayment([FromBody] DeletedPayment deletedPayment)
        {
            var bsp = this.branchStudentPayment.GetById(deletedPayment.Id);
            var deleted = this.mapper.Map<BranchStudentPayment, DeletedPayment>(bsp, deletedPayment);
            this.branchStudentPayment.Remove(bsp, deleted);
            return Ok();
        }


    }
}