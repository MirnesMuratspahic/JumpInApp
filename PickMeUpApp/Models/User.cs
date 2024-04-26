using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PickMeUpApp.Models
{
    public class User
    {
        [JsonIgnore] [Key] public int Id { get; set; }    
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [JsonIgnore] public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } =string.Empty;

    }
}
