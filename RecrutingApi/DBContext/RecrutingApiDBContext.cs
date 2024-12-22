using Microsoft.EntityFrameworkCore;
using RecrutingApi.Model;

namespace RecrutingApi.DBContext
{
    public class RecrutingApiDBContext(DbContextOptions<RecrutingApiDBContext> options) : DbContext(options)
    {
        public DbSet<Users> users { get; set; }

        public DbSet<Document> documents { get; set; }
        public DbSet<Candidate> candiates { get; set; }
        public DbSet<Recruiter> recruiters { get; set; }
    }
}