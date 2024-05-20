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
            var email = "superadmin@lms.com";
            builder.HasData
            (
                new SystemUser
                {
                    Id = 1,
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = email,
                    NormalizedUserName = email.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    RoleId = 1,
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    SecurityStamp = "initial",
                    ConcurrencyStamp = "initial",
                    PasswordHash = "AQAAAAIAAYagAAAAEB96RXidUA3MA/QqigqlV2OEbUJsIUdP64w37HPYRKtMxGh2qU2SvC6BAS08KuS0Yw=="
                }
            );
            builder.HasOne(u => u.Role)
                .WithMany(r => r.SystemUsers)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}