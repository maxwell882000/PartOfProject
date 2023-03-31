using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VticLibrary.Models;

namespace VitcAuth.Models;

public partial class User : IdentityUser<long>, IModel
{
    public long? CompanyId { get; set; }

    public long? RegionId { get; set; }

    public long? BranchId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? RememberToken { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? OrganizationId { get; set; }

    public long? RoleId { get; set; }

    public string Lang { get; set; } = null!;

    public int verificationCode { get; set; }

    public bool? Status { get; set; }

    public long? CategoryId { get; set; }

    public long? GroupId { get; set; }

    public int? EducationLevel { get; set; }

    public bool FtestPermission { get; set; }

    public long? SystemStatus { get; set; }

    public bool IsPreferential { get; set; }

    public int Preferential { get; set; }

    public long? StudentTypeId { get; set; }

    public long? SubBranchId { get; set; }

    public int? Graduated { get; set; }

    public bool AccountantPermission { get; set; }

    public long Period { get; set; }

    public long? CustomId { get; set; }

    [ForeignKey("RegionId")]
    public virtual Region? Region { get; set; }

    [ForeignKey("OrganizationId")]
    public virtual RegionOrganization? Organization { get; set; }

    [ForeignKey("BranchId")]
    public virtual Branch? Branch { get; set; }

    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }
    public virtual IList<UserContact> Contacts { get; } = new List<UserContact>();

    [ForeignKey("RoleId")]
    public virtual Role? Role { get; set; }
    public virtual List<BranchStudentPayment> BranchStudentPayments { get; set; }

    public virtual ICollection<Employee> Employees { get; } = new List<Employee>();

    public virtual Group? Group { get; set; }

    public virtual PaymentRest? PaymentRest { get; set; }


    public virtual ICollection<UserFinalTestResult> UserFinalTestResults { get; } = new List<UserFinalTestResult>();

    public virtual UserPassportDatum? Passport { get; set; }

    public virtual UserProfile? Profile { get; set; }

    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    public virtual ICollection<Role> Roles { get; set; }

    [JsonIgnore]
    public virtual List<UserRole> UserRoles { get; set; }

    public virtual UserProfilesReduce? UserProfilesReduce { get; set; }

    public virtual ICollection<VisitTimeStat> VisitTimeStats { get; } = new List<VisitTimeStat>();

    public virtual List<SystemStatusStep> SystemStatusSteps { get; }



    [ForeignKey("SystemStatus")]
    public virtual SystemStatus? SystemStatusEntity { get; set; }

    [NotMapped]
    [JsonIgnore]
    public bool isSystemAccessible
    {
        get
        {
            return this.SystemStatus == null ? true : this?.SystemStatusEntity?.SystemAccess == true;
        }
    }

    [NotMapped]
    public bool isStudent { get { return RoleId == 7; } }

    public User()
    {

    }

    private ILazyLoader LazyLoader { get; set; }

    public User(ILazyLoader lazyLoader)
    {
        LazyLoader = lazyLoader;
    }

    public void getRelated(string related)
    {
        this.LazyLoader.Load(this, related);
    }

    [NotMapped]
    public TeacherType? TeacherType { get => TeacherTypes.LastOrDefault(); }

    [NotMapped]
    public virtual List<TeacherCurrentType>? TeacherCurrentType { get; set; }
    public virtual ICollection<TeacherType> TeacherTypes { get; set; } = new List<TeacherType>();

    public virtual ICollection<Group> TaughtGroup { get; set; }

    public virtual OwnerSecret UserSecret { get; set; }

    public virtual List<UserLicense> UserLicenses { get; set; }

    public virtual List<UserLicenseCategory> UserLicenseCategories { get; set; }

    public virtual List<PreferentialStudent> PreferentialStudents { get; set; }

    public virtual StudentCertificate StudentCertificate { get; set; }
}
