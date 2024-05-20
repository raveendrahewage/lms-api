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
                new SystemRole { Id = 1, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "initial" },
                new SystemRole { Id = 3, Name = "User", NormalizedName = "USER", ConcurrencyStamp = "initial" }
            );
        }
    }
}