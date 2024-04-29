using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PickMeUpApp.Models.DTO
{
    public class dtoUser
    {
        [Required]
        public string UserToken { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
