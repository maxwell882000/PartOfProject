using VitcAuth.Models;

namespace VitcAuth.Controllers.Moderator.Auto
{

    public class GroupEditResponse
    {
        public long Id { get; set; }
        public string GroupName { get; set; }
        public double? Price { get; set; }

        public DateOnly? StartDate { get; set; }
        public virtual StudyPeriod Period { get; set; }

        public virtual Branch? Branch { get; set; }

        public virtual Category? Category { get; set; }
        public UserCredResponse? MainTeacher { get; set; }

        public UserCredResponse? MainAssistant { get; set; }

        public UserCredResponse? MedTeacher { get; set; }

        public virtual ICollection<UserCredResponse> PracticeTeachers { get; set; }
    }
}