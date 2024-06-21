using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ACIS.Data.Configurations
{
    public class DateWiseLeavesConfiguration : IEntityTypeConfiguration<DateWiseLeave>
    {
        public void Configure(EntityTypeBuilder<DateWiseLeave> builder)
        {
            builder.HasOne(u => u.Leave)
                .WithMany(r => r.DateWiseLeaves)
                .HasForeignKey(u => u.LeaveId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasQueryFilter(x => x.Status == DataRecordStatus.Active);
        }
    }
}