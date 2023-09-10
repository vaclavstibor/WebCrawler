using Microsoft.EntityFrameworkCore;
using WebCrawler.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace WebCrawler.DataAccessLayer.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }
        }

        public DbSet<WebsiteRecord> Records { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<StartingNode> StartingNodes { get; set; }
        public DbSet<Execution> Executions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
    
}
