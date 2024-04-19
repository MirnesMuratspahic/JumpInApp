namespace PickMeUpApp.Models.DTO
{
    public class dtoUserRoute
    {
        public dtoUser User { get; set; }
        public dtoTheRoute Route { get; set; }

        public dtoUserRoute() { }
        public dtoUserRoute(User user, TheRoute route)
        {
            User = new dtoUser(user);
            Route = new dtoTheRoute(route);
        }
    }
}
