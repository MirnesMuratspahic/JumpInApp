using System.ComponentModel.DataAnnotations;

namespace PickMeUpApp.Models.DTO
{
    public class dtoRequestRecived
    {
        [Required]
        public string UserToken { get; set; }
        [Required]
        public Request Request { get; set; }

        public dtoRequestRecived() { }
    }
}
