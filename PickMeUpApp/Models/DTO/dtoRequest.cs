using System.ComponentModel.DataAnnotations;

namespace PickMeUpApp.Models.DTO
{
    public class dtoRequest
    {
        [Required]
        public dtoUserRoute dtoUserRoute { get; set; }
        [Required]
        public string passengerEmail { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = "Panding";
    }
}
