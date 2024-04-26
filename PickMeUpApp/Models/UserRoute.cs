using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PickMeUpApp.Models
{
    public class UserRoute
    {
        [JsonIgnore] [Key] public int Id { get; set; }
        [Required]
        [JsonIgnore] public int UserId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        [JsonIgnore] public int RouteId { get; set; }
        [Required]
        public TheRoute Route { get; set; }
    }
}
