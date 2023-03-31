using VitcAuth.Models;

namespace VitcAuth.Controllers.Moderator.Auto
{
    public class GroupOnlyResponse : Group
    {
        public long Id { get; set; }

        public string GroupName { get; set; }
        public bool NotGroup { get; set; } = false;
    }
}