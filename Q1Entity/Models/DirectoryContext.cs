using System;
using System.Configuration;
using System.IO;
using Microsoft.EntityFrameworkCore;


namespace Q1Entity
{
    public class DirectoryContext : DbContext
    {

        public DbSet<DirectoryItem> DirectoryItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserDirectoryItem> UserDirectoryItems { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // store db file at sqlitePath
            var sqlitePath = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData),
                "pulsenics");
            // check if the directory exists at sqlitePath, If not, create it
            if (!Directory.Exists(sqlitePath)) 
                Directory.CreateDirectory(sqlitePath);
            optionsBuilder.UseSqlite($"Data Source={Path.Combine(sqlitePath, "TechAssessmentQ1.db")}");
            base.OnConfiguring(optionsBuilder);
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
