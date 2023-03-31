namespace VitcAuth.Controllers.Moderator.Auto
{
    public class UserCredResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
        public TeacherTypeCred? TeacherType { get; set; }

        public long? SystemStatus { get; set; }
    }
}