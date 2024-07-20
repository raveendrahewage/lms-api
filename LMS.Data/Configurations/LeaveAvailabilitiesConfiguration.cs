using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ACIS.Data.Configurations
{
    public class LeaveAvailabilitiesConfiguration : IEntityTypeConfiguration<LeaveAvailability>
    {
        public void Configure(EntityTypeBuilder<LeaveAvailability> builder)
        {
            builder.HasOne(u => u.SystemUser)
                .WithMany(r => r.LeaveAvailabilities)
                .HasForeignKey(u => u.SystemUserId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(u => u.LeaveType)
                .WithMany(r => r.LeaveAvailabilities)
                .HasForeignKey(u => u.LeaveTypeId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasQueryFilter(x => x.Status == DataRecordStatus.Active);
        }
    }
}