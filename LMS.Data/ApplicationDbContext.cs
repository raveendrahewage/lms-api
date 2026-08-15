using ACIS.Data.Configurations;
using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<SystemUser, SystemRole, int>(options)
    {
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<DateWiseLeave> DateWiseLeaves { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<LeaveAvailability> LeaveAvailabilities { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration<SystemRole>(new SystemRolesConfiguration());
            modelBuilder.ApplyConfiguration<SystemUser>(new SystemUsersConfiguration());
            modelBuilder.ApplyConfiguration<IdentityUserRole<int>>(new SystemUserRolesConfiguration());
            modelBuilder.ApplyConfiguration<Leave>(new LeavesConfiguration());
            modelBuilder.ApplyConfiguration<DateWiseLeave>(new DateWiseLeavesConfiguration());
            modelBuilder.ApplyConfiguration<LeaveType>(new LeaveTypesConfiguration());
            modelBuilder.ApplyConfiguration<Event>(new EventsConfiguration());
            modelBuilder.ApplyConfiguration<Notification>(new NotificationConfiguration());
        }
    }
}
