using System.ComponentModel.DataAnnotations.Schema;
using VitcAuth.Models;
using VticLibrary.Models;

namespace VitcAuth.Controllers.Moderator.Auto
{
    public class UserEdit : MediaModel
    {
        public long Id { get; set; }
        public IFormFile? Avatar { get; set; }
        public long? RegionId { get; set; }

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;
        public int Preferential { get; set; }

        public long? CategoryId { get; set; }
        public long? StudentTypeId { get; set; }
        public string? MedCardNumber { get; set; }
        public int? EducationLevel { get; set; }

        public string? Address { get; set; }

        public UserEdit()
        { }
        public long Period { get; set; }

        public override IFormFile media { get => this.Avatar; }

        public virtual List<UserLicense> UserLicenses { get; set; }

        public virtual List<UserLicenseCategory> UserLicenseCategories { get; set; }
        public override string path => "apiv2/user/avatars";
        public virtual List<PreferentialStudent> PreferentialStudents { get; set; }

        public virtual ICollection<UserContact> Contacts { get; } = new List<UserContact>();

    }
    public class UserCreate : UserEdit
    {
        public UserCreate()
        { }

        public bool? Gender { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Surname { get; set; }
        [NotMapped]

        public DateOnly? Birthdate { get; set; }




        public long? CompanyId { get; set; }


        public long? BranchId { get; set; }




        public string? RememberToken { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public long? OrganizationId { get; set; }

        public long? RoleId { get; set; }

        public string Lang { get; set; } = null!;

        public int verificationCode { get; set; }

        public bool? Status { get; set; }


        public long? GroupId { get; set; }


        public bool FtestPermission { get; set; }

        public long? SystemStatus { get; set; }

        public bool IsPreferential { get; set; }



        public long? SubBranchId { get; set; }

        public int? Graduated { get; set; }

        public bool AccountantPermission { get; set; }


        public long? CustomId { get; set; }

        [ForeignKey("RegionId")]
        public virtual Region? Region { get; set; }

        [ForeignKey("OrganizationId")]
        public virtual RegionOrganization? Organization { get; set; }

        [ForeignKey("BranchId")]
        public virtual Branch? Branch { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }
        public virtual List<BranchStudentPayment> BranchStudentPayments { get; set; }

        public virtual ICollection<Employee> Employees { get; } = new List<Employee>();

        public virtual Group? Group { get; set; }

        public virtual PaymentRest? PaymentRest { get; set; }

        public virtual StudyPeriod PeriodNavigation { get; set; } = null!;

        public virtual ICollection<UserFinalTestResult> UserFinalTestResults { get; } = new List<UserFinalTestResult>();

        public virtual UserPassportDatum? Passport { get; set; }

        public virtual UserProfile? Profile { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company? Company { get; set; }
        public virtual ICollection<Role> Roles { get; set; }


        public virtual UserProfilesReduce? UserProfilesReduce { get; set; }

        public virtual ICollection<VisitTimeStat> VisitTimeStats { get; } = new List<VisitTimeStat>();

        public virtual List<SystemStatusStep> SystemStatusSteps { get; }



        [ForeignKey("SystemStatus")]
        public virtual SystemStatus? SystemStatusEntity { get; set; }


        [NotMapped]
        public bool isStudent { get { return RoleId == 7; } }




        [NotMapped]
        public TeacherType? TeacherType { get => TeacherTypes.LastOrDefault(); }

        public virtual List<TeacherCurrentType>? TeacherCurrentType { get; set; }
        public virtual ICollection<TeacherType> TeacherTypes { get; set; } = new List<TeacherType>();

        public virtual ICollection<Group> TaughtGroup { get; set; }

        public virtual OwnerSecret UserSecret { get; set; }


    }
}