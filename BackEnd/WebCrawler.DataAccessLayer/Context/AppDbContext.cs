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
                //optionsBuilder.UseSqlServer();
                optionsBuilder.UseSqlServer("Server=127.0.0.1,1401;Initial Catalog=MyDB;User Id=SA;Password=&VeryComplex123Password;TrustServerCertificate=True");
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
