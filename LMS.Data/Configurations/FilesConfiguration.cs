using LMS.Data.Enum;
using LMS.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using File = LMS.Data.Models.File;

namespace ACIS.Data.Configurations
{
    public class FilesConfiguration : IEntityTypeConfiguration<File>
    {
        public void Configure(EntityTypeBuilder<File> builder)
        {
            builder.HasOne(u => u.UploadedBy)
                .WithMany(r => r.Files)
                .HasForeignKey(u => u.UploadedById)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasQueryFilter(x => x.Status == DataRecordStatus.Active);
        }
    }
}