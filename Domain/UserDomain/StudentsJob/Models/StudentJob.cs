using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VticLibrary.Models;

namespace VitcAuth.Models;

public partial class StudentJob : IModel
{
    public long Id { get; set; }

    public long? CompanyId { get; set; }

    public long? RegionId { get; set; }

    public long? BranchId { get; set; }

    public long? OrganizationId { get; set; }

    public long? JobId { get; set; }

    public long? GroupId { get; set; }

    public int Status { get; set; }

    public string Username { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Surname { get; set; }

    public string Lang { get; set; } = null!;

    public string? Avatar { get; set; }

    public int? EducationLevel { get; set; }

    public int? SystemStatus { get; set; }

    public bool? Preferential { get; set; }

    public bool? Gender { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? MedCardNumber { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? SubBranchId { get; set; }

    public int? PreferentialId { get; set; }

    public long? EducationPrice { get; set; }

    public virtual JobPaymentRest? JobPaymentRest { get; set; }

    public virtual List<JobStudentPayment> Payment { get; set; } = new List<JobStudentPayment>();

    public virtual StudentJobPassport StudentJobPassport { get; set; }

    public virtual List<StudentJobContact> StudentJobContacts { get; set; }
    public virtual List<JobSystemStatusHistory> SystemStatusHistories { get; }

    [ForeignKey("JobId")]
    public virtual Job Job { get; set; }
    [ForeignKey("GroupId")]
    public virtual ProfessionGroup? Group { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }
    [ForeignKey("RegionId")]
    public virtual Region? Region { get; set; }


    private ILazyLoader LazyLoader { get; set; }

    public StudentJob(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    public void getRelated(string related)
    {
        this.LazyLoader.Load(this, related);
    }
}
