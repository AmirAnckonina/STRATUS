using Microsoft.EntityFrameworkCore;
using StratusApp.Models;
using CloudApiClient.DTO;

namespace StratusApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<StratusUser> Users { get; set; }

        public DbSet<AlternativeInstance> AlternativeInstances { get; set; }
    }
}
