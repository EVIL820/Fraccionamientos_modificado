using Fraccionamientos_LDS.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fraccionamientos_LDS.Data
{
    public class ResidentialContext : DbContext
    {
        public ResidentialContext(DbContextOptions<ResidentialContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
