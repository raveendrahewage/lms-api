using LMS.Data.CoreIdentity;
using LMS.Data.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ACIS.Data.Configurations
{
    public class CoreIdentityRolesConfiguration : IEntityTypeConfiguration<CoreIdentityRole>
    {
        public void Configure(EntityTypeBuilder<CoreIdentityRole> builder)
        {
            builder.HasData
            (
                new CoreIdentityRole { Id = 1, Name = SysRoles.Admin, NormalizedName = SysRoles.Admin.ToUpper() },
                new CoreIdentityRole { Id = 3, Name = SysRoles.User, NormalizedName = SysRoles.User.ToUpper() }
            );
        }
    }
}