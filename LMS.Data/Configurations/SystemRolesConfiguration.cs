using LMS.Data.CoreIdentity;
using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ACIS.Data.Configurations
{
    public class SystemRolesConfiguration : IEntityTypeConfiguration<SystemRole>
    {
        public void Configure(EntityTypeBuilder<SystemRole> builder)
        {
            builder.HasData
            (
                new SystemRole { Id = 1, Name = "Admin", FrameworkRoleId = 1 },
                new SystemRole { Id = 2, Name = "Supervisor", FrameworkRoleId = 2 },
                new SystemRole { Id = 3, Name = "User", FrameworkRoleId = 3 }
            );
        }
    }
}