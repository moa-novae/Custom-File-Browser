using System.Configuration;
using Microsoft.EntityFrameworkCore;



namespace Entity
{
    public class DirectoryContext : DbContext
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
            modelBuilder.Entity<UserDirectoryItem>()
                .HasKey(ud => new { ud.UserId, ud.DirectoryItemId });
            modelBuilder.Entity<UserDirectoryItem>()
                .HasOne(ud => ud.User)
                .WithMany(d => d.UserDirectoryItems)
                .HasForeignKey(ud => ud.UserId);
            modelBuilder.Entity<UserDirectoryItem>()
                .HasOne(ud => ud.DirectoryItem)
                .WithMany(u => u.UserDirectoryItems)
                .HasForeignKey(ud => ud.DirectoryItemId);
            base.OnModelCreating(modelBuilder);

        }

    }
}
