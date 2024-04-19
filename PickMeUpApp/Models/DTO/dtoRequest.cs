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

        public dtoRequest() { }

        public dtoRequest(Request request)
        {
            dtoUserRoute = new dtoUserRoute(request.UserRoute.User, request.UserRoute.Route);
            passengerEmail = request.PassengerEmail;
            Description = request.Description;
            Status = request.Status;
        }

        public dtoRequest(UserRoute _dtoUserRoute, string passengerEmail, string description, string status)
        {
            this.dtoUserRoute = new dtoUserRoute(_dtoUserRoute.User, _dtoUserRoute.Route);
            this.passengerEmail = passengerEmail;
            Description = description;
            Status = status;
        }
    }
}
