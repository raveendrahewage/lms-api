using LMS.Data.CoreIdentity;
using LMS.Data.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ACIS.Data.Configurations
{
    public class CoreIdentityUsersConfiguration : IEntityTypeConfiguration<CoreIdentityUser>
    {
        public void Configure(EntityTypeBuilder<CoreIdentityUser> builder)
        {
            builder.HasData
            (
                new CoreIdentityUser
                {
                    Id = 1,
                    UserName = "superuser@lml.com",
                    NormalizedUserName = "superuser@lml.com",
                    Email = "superuser@lml.com",
                    NormalizedEmail = "superuser@lml.com",
                    EmailConfirmed = true,
                    LockoutEnabled = true,
                    SecurityStamp = "initial",
                    ConcurrencyStamp = "initial",
                    PasswordHash = "AQAAAAEAACcQAAAAEO/OKTaYnFYnbrcZ/1oFdpX4j611YcimIIs+/PgcQbaQHX/LK9RtC1IpnPsZxMonJw=="
                }
            );
        }
    }
}