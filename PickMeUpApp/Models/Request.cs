using PickMeUpApp.Models.DTO;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PickMeUpApp.Models
{
    public class Request
    {
        [JsonIgnore] [Key] public int Id { get; set; }
        [Required] 
        public UserRoute UserRoute { get; set; }
        [Required]
        public string PassengerEmail { get; set; } = string.Empty;
        [Required] 
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = string.Empty;

        
    }
}
