using System.ComponentModel.DataAnnotations;

namespace PickMeUpApp.Models.DTO
{
    public class dtoTheRoute
    {
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

        public dtoTheRoute() { }
        public dtoTheRoute(TheRoute route) 
        {
            Name = route.Name;
            Description = route.Description;
            SeatsNumber = route.SeatsNumber;
            DateAndTime = route.DateAndTime;
            Price = route.Price;
        }
    }
}
