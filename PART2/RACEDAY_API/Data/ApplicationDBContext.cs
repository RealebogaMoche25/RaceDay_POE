using System;
using Microsoft.EntityFrameworkCore;

namespace RACEDAY_API.Data
{
    public class ApplicationDBContext : DbContext
{
	public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options): base(options)
	{
	}
}
