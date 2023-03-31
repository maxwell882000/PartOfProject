using VitcAuth.Models;

namespace VitcAuth.Controllers.Moderator.Auto
{
    public class GroupEditResponseWithUsers : GroupEditResponse
    {
        public UserCredResponse? PracticeTeacher { get; set; }

        public UserCredResponse? PracticeAssistantTeacher { get; set; }
    }
}