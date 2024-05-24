using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ACIS.Data.Configurations
{
    public class LeaveTypesConfiguration : IEntityTypeConfiguration<LeaveType>
    {
        public void Configure(EntityTypeBuilder<LeaveType> builder)
        {
            builder.HasMany(u => u.Leaves)
                .WithOne(r => r.LeaveType)
                .HasForeignKey(u => u.LeaveTypeId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasQueryFilter(x => x.Status == DataRecordStatus.Active);
        }
    }
}