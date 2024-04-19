using System.ComponentModel.DataAnnotations;

namespace PickMeUpApp.Models.DTO
{
    public class dtoTheRoute
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;

        public dtoTheRoute() { }
        public dtoTheRoute(TheRoute route) 
        {
            Name = route.Name;
            Description = route.Description;
        }
    }
}
