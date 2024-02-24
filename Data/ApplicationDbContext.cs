using InvestmentAdvisory.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentAdvisory.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Advisor> Advisors { get; set; }

        public DbSet<Client> Clients { get; set; }
    }
}
