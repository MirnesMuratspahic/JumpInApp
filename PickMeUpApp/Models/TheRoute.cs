using System.ComponentModel.DataAnnotations;

namespace PickMeUpApp.Models
{
    public class TheRoute
    {
        [Key] public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int SeatsNumber { get; set; }
        [Required]
        public DateTime DateAndTime { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
    }
}
