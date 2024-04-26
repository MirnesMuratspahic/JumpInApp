using System.ComponentModel.DataAnnotations;

namespace PickMeUpApp.Models.DTO
{
    public class dtoRequestSent
    {
        [Required]
        public UserRoute UserRoute { get; set; }
        [Required]
        public string passengerToken { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; }

        public dtoRequestSent() { }

    }
}
