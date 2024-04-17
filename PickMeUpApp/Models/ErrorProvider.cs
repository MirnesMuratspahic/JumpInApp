using System.ComponentModel.DataAnnotations.Schema;

namespace PickMeUpApp.Models
{
    [NotMapped]
    public class ErrorProvider
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; } = false;
    }
}
