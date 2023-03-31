using VitcAuth.Models;

namespace VitcAuth.Controllers.Moderator.Auto
{
    public class CredentialResponse
    {
        public List<Category> Categories { get; set; }
        public List<UserCredResponse> Teachers { get; set; }
        public List<StudyPeriod> Periods { get; set; }

        public List<BranchCategoryPrice> Prices { get; set; }

        public List<Branch> Branchs { get; set; }

        public UserCounter Login { get; set; }

        public GroupEditResponse Group { get; set; }

    }
}