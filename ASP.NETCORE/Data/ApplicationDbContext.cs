using ASP.NETCORE.Models;
using Microsoft.EntityFrameworkCore;
namespace ASP.NETCORE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Student> Students { get; set; }
        public DbSet<Models.Department> Departments { get; set; }
        public DbSet<AppUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-many relationship between Department and Student
            modelBuilder.Entity<Models.Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure required foreign key
            modelBuilder.Entity<Models.Student>()
                .Property(s => s.DepartmentId)
                .IsRequired();
        }
    }
}
