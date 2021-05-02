using Microsoft.EntityFrameworkCore;
using NeighborHelp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NeighborHelp.Services
{
    public class ApplicationContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Order> Orders { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options, bool clearOldData = false) :base(options)
        {
            if (clearOldData)
            {
                Database.EnsureDeleted();
            }
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
            .HasOne(u => u.Profile).WithOne(p => p.OwnerUser)
            .HasForeignKey<UserProfile>(up => up.Id);

            modelBuilder.Entity<Order>()
               .HasOne(o => o.Author).WithMany(a => a.Orders)
               .HasForeignKey(o => o.AuthorId);
        }
    }
}
