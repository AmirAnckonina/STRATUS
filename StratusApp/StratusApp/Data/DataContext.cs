using Microsoft.EntityFrameworkCore;
using StratusApp.Models;
using Utils.DTO;

namespace StratusApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Utils.DTO.StratusUser> Users { get; set; }

        public DbSet<AlternativeInstance> AlternativeInstances { get; set; }
    }
}
