using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitcAuth.Collections.Interfaces;
using VitcAuth.DTO.Payments;
using VitcAuth.Models;
using VitcAuth.Repository.Interfaces;

namespace VitcAuth.Controllers.Accountant
{
    [Authorize]
    [ApiController]
    [Route("apiv2/[controller]")]
    public class JobPaymentController : ControllerBase
    {
        private IStudentJobRepository studentJob;
        private IUserRepository userRepository;
        private IBranchStudentPaymentStatusRepository branchStudentPaymentStatus;
        private IJobPaymentRepository paymentRepository;

        private IPaymentCollection paymentCollection;
        private IMapper mapper;
        public JobPaymentController(
            IStudentJobRepository studentJob,
            IJobPaymentRepository jobPaymentRepository,
            IBranchStudentPaymentStatusRepository branchStudentPaymentStatus,
            IPaymentCollection paymentCollection,
            IUserRepository userRepository,
            IMapper mapper)
        {
            this.studentJob = studentJob;
            this.userRepository = userRepository;
            this.branchStudentPaymentStatus = branchStudentPaymentStatus;
            this.paymentRepository = jobPaymentRepository;
            this.paymentCollection = paymentCollection;
        }
        [HttpPost("changeEducationPrice")]
        public IActionResult changeEducationPrice(ChangeStudentPrice change)
        {
            if (change.Price > 0)
            {
                var student = this.studentJob.GetById(change.StudentId);
                student.EducationPrice = change.Price;
                this.studentJob.update(student);
                this.studentJob.commit();
                return Ok(new { data = student });
            }
            this.ModelState.AddModelError("error", "Not given price");
            return ValidationProblem();
        }

        [HttpDelete("removePayment")]
        public IActionResult removePayment([FromQuery] long id)
        {
            var payment = this.paymentRepository.GetById(id);
            this.paymentRepository.Remove(payment);
            return Ok(new { data = "deleted" });
        }

        [HttpPost("makePayment")]
        public IActionResult makePayment([FromBody] CreateKasbPayment kasbPayment)
        {
            var payment = this.paymentRepository.Add(kasbPayment);
            return Ok(new { data = payment });
        }
    }
}