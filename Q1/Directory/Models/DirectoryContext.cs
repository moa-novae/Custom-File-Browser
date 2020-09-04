using System.Configuration;
using Microsoft.EntityFrameworkCore;


namespace Q1
{
    class DirectoryContext : DbContext
    {

        public DbSet<DirectoryItem> DirectoryItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDirectoryItem> UserDirectoryItems { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["TechAssessmentQ1"].ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDirectoryItem>().HasKey(sc => new { sc.UserId, sc.DirectoryItemId });
        }

    }
}
