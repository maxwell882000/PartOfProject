namespace VitcAuth.Controllers.Moderator.Auto
{
    public class GroupShowResponse : GroupEditResponse
    {
        public DateOnly? EndDate { get; set; }
        public int Status { get; set; }
        public long CategoryId { get; set; }

        public int LessonVersion { get; set; }
        public int CurriculumCount { get; set; }
    }
}