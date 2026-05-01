using Learnova.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Persistence.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Progress).HasDefaultValue(0);
            
            builder.HasOne(x => x.Course).WithMany(c => c.Enrollments).HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Student).WithMany(c => c.Enrollments).HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.StudentId, e.CourseId }).IsUnique();
        }
    }
}
