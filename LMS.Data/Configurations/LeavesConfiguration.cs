using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ACIS.Data.Configurations
{
    public class LeavesConfiguration : IEntityTypeConfiguration<Leave>
    {
        public void Configure(EntityTypeBuilder<Leave> builder)
        {
            builder.HasOne(u => u.LeaveType)
                .WithMany(r => r.Leaves)
                .HasForeignKey(u => u.LeaveTypeId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(u => u.Employee)
                .WithMany(r => r.Leaves)
                .HasForeignKey(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(u => u.Reviewer)
                .WithMany(r => r.ReviewedLeaves)
                .HasForeignKey(u => u.ReviewedBy)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(u => u.DateWiseLeaves)
                .WithOne(r => r.Leave)
                .HasForeignKey(u => u.LeaveId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.Property(e => e.Version)
                .IsRequired()
                .IsRowVersion()
                .IsConcurrencyToken()
                .ValueGeneratedOnAddOrUpdate();
            builder.HasQueryFilter(x => x.Status == DataRecordStatus.Active);
        }
    }
}