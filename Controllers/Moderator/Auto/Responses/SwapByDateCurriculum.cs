namespace VitcAuth.Controllers.Moderator.Auto
{
    public class SwapByDateCurriculum
    {
        public long GroupId { get; set; }
        public DateOnly Old { get; set; }
        public DateOnly New { get; set; }
    }
}