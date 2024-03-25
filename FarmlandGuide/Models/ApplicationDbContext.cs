using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarmlandGuide.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FarmlandGuide.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public virtual DbSet<ProductionProcess> ProductionProcesses { get; set; } = null!;
        public virtual DbSet<Enterprise> Enterprises { get; set; } = null!;
        public virtual DbSet<WorkSession> WorkSessions { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public ApplicationDbContext() => Database.EnsureCreated();
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
: base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Укажите строку подключения к вашей базе данных
            optionsBuilder.UseSqlite("Data Source=FarmlandGuide2.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeID);
                entity.HasOne(e => e.Enterprise).WithMany(e => e.Employees);
                entity.HasMany(e => e.Tasks).WithOne(t => t.Employee);
                entity.HasMany(e => e.WorkSessions).WithOne(ws => ws.Employee);
                entity.HasOne(e => e.Role).WithMany(r => r.Employees);
            });

            modelBuilder.Entity<Enterprise>(entity =>
            {
                entity.HasKey(e => e.EnterpriseID);
                entity.HasMany(e => e.Employees).WithOne(e => e.Enterprise);
                entity.HasMany(e => e.ProductionProcesses).WithOne(pp => pp.Enterprise);
            });

            modelBuilder.Entity<ProductionProcess>(entity =>
            {
                entity.HasKey(e => e.ProcessID);
                entity.HasMany(pp => pp.Tasks).WithOne(t => t.ProductionProcess);
                entity.HasOne(p => p.Enterprise).WithMany(e => e.ProductionProcesses);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleID);
                entity.HasMany(r => r.Employees).WithOne(e => e.Role);
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.HasKey(t => t.TaskID);
                entity.HasOne(t => t.Employee).WithMany(e => e.Tasks);
                entity.HasOne(t => t.ProductionProcess).WithMany(pc => pc.Tasks);
                entity.HasOne(t => t.Status).WithMany(s => s.Tasks);
            });
            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(s => s.StatusID);
                entity.HasMany(s => s.Tasks).WithOne(t => t.Status);
            });

            modelBuilder.Entity<WorkSession>(entity =>
            {
                entity.HasKey(ws => ws.SessionID);
                entity.HasOne(ws => ws.Employee).WithMany(e => e.WorkSessions).OnDelete(DeleteBehavior.Cascade);
            });


        }
    }
}
