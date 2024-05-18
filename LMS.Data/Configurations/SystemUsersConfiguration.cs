using LMS.Data.CoreIdentity;
using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ACIS.Data.Configurations
{
    public class SystemUsersConfiguration : IEntityTypeConfiguration<SystemUser>
    {
        public void Configure(EntityTypeBuilder<SystemUser> builder)
        {
            builder.HasData
            (
                new SystemUser
                {
                    Id = 1,
                    FirstName = "Super",
                    LastName = "Admin",
                    FrameworkRoleId = 1,
                    Email = "superadmin@lms.com",
                    FrameworkUserId = 1
                }
            );
        }
    }
}