using FarmlandGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmlandGuide.Models
{
    public sealed class ApplicationDbContext : DbContext
    {
        public DbSet<ProductionProcess> ProductionProcesses { get; set; } = null!;
        public DbSet<Enterprise> Enterprises { get; set; } = null!;
        public DbSet<WorkSession> WorkSessions { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<Task> Tasks { get; set; } = null!;
        public DbSet<Status> Statuses { get; set; } = null!;
        public ApplicationDbContext() => Database.EnsureCreated();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
: base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Укажите строку подключения к вашей базе данных
#if DEBUG
            optionsBuilder.UseSqlite("Data Source=FarmlandGuide2.db");
#else
            optionsBuilder.UseSqlite("Data Source=FarmlandGuide.ctx");
#endif
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);
                entity.HasOne(e => e.Enterprise).WithMany(e => e.Employees);
                entity.HasMany(e => e.Tasks).WithOne(t => t.Employee);
                entity.HasMany(e => e.WorkSessions).WithOne(ws => ws.Employee);
                entity.HasOne(e => e.Role).WithMany(r => r.Employees);
            });

            modelBuilder.Entity<Enterprise>(entity =>
            {
                entity.HasKey(e => e.EnterpriseId);
                entity.HasMany(e => e.Employees).WithOne(e => e.Enterprise);
                entity.HasMany(e => e.ProductionProcesses).WithOne(pp => pp.Enterprise);
            });

            modelBuilder.Entity<ProductionProcess>(entity =>
            {
                entity.HasKey(e => e.ProcessId);
                entity.HasMany(pp => pp.Tasks).WithOne(t => t.ProductionProcess);
                entity.HasOne(p => p.Enterprise).WithMany(e => e.ProductionProcesses);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId);
                entity.HasMany(r => r.Employees).WithOne(e => e.Role);
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasKey(t => t.TaskId);
                entity.HasOne(t => t.Employee).WithMany(e => e.Tasks);
                entity.HasOne(t => t.ProductionProcess).WithMany(pc => pc.Tasks);
                entity.HasOne(t => t.Status).WithMany(s => s.Tasks);
            });
            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(s => s.StatusId);
                entity.HasMany(s => s.Tasks).WithOne(t => t.Status);
            });

            modelBuilder.Entity<WorkSession>(entity =>
            {
                entity.HasKey(ws => ws.SessionId);
                entity.HasOne(ws => ws.Employee).WithMany(e => e.WorkSessions).OnDelete(DeleteBehavior.Cascade);
            });


        }
    }
}
