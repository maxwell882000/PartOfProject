using VitcAuth.Models;
using VticLibrary.Models;

namespace VitcAuth.Controllers.Moderator.Kasb
{
    public class StudentKasbCreate : MediaModel
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

        public IFormFile? Avatar { get; set; }

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

    
        public virtual StudentJobPassport StudentJobPassport { get; set; }

        public virtual List<StudentJobContact> StudentJobContacts { get; set; }

        public override IFormFile media =>  this.Avatar;

        public override string path => "apiv2/user/avatars/kasb";
    }
}