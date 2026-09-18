using RACEDAY_API.Models;
using Microsoft.EntityFrameworkCore;

namespace RACEDAY_API.Data
{
	public class ApplicationDBContext : DbContext
	{
		public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
		{
		}

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Enrolment> Enrolments { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Result> Results { get; set; }

        public DbSet<Routes> Routes { get; set; }

    }
}