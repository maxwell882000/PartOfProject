using VitcAuth.Models;

namespace VitcAuth.Controllers.Moderator.Auto
{
    public class CalculatePriceResponse : UserProfile
    {
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}