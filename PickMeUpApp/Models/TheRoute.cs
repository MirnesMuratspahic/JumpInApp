using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PickMeUpApp.Models
{
    public class TheRoute
    {
        [JsonIgnore] [Key] public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public int SeatsNumber { get; set; }
        [Required]
        public string DateAndTime { get; set; } 
        [Required]
        public float Price { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public string Type { get; set; } = string.Empty;
    }
}
