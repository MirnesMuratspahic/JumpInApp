using System.ComponentModel.DataAnnotations;

namespace PickMeUpApp.Models
{
    public class UserRoute
    {
        [Key] public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public int RouteId { get; set; }
        [Required]
        public TheRoute Route { get; set; }
    }
}
