using ACIS.Data.Configurations;
using LMS.Data.CoreIdentity;
using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Data
{
    public class ApplicationDbContext: IdentityDbContext<CoreIdentityUser, CoreIdentityRole, int>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<SystemUser> SystemUsers { get; set; }
        public DbSet<SystemRole> SystemRoles { get; set; }
        public DbSet<Leaves> Leaves { get; set; }
        public DbSet<LeaveType> LeaveTypes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration<CoreIdentityRole>(new CoreIdentityRolesConfiguration());
            modelBuilder.ApplyConfiguration<CoreIdentityUser>(new CoreIdentityUsersConfiguration());
            modelBuilder.ApplyConfiguration<SystemRole>(new SystemRolesConfiguration());
            modelBuilder.ApplyConfiguration<SystemUser>(new SystemUsersConfiguration());
        }
    }
}
