using Microsoft.EntityFrameworkCore;
using StratusApp.Models;

namespace StratusApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {

        }

        public DbSet<StratusUser> Users { get; set; }   
    }
}
