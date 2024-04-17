using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PickMeUpApp.Models;

namespace PickMeUpApp.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users {  get; set; }
        public DbSet<TheRoute> Routes { get; set; }
        public DbSet<UserRoute> UserRoutes { get; set; }

    }

}
