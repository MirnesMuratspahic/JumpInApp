using System.ComponentModel.DataAnnotations;

namespace PickMeUpApp.Models
{
    public class User
    {
        [Key] public int UserId { get; set; }    
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } =string.Empty;

    }
}
