namespace VitcAuth.Controllers.Moderator.Auto
{
    public class ScheduleResponse
    {

        public long Id { get; set; }
        public long GroupId { get; set; }
        public string LessonName { get; set; }
        public string TeacherTypeName { get; set; }
        public DateOnly Date { get; set; }



    }
}