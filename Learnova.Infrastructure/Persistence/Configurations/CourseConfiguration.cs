using Learnova.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Persistence.Configurations
{
    public class CourseConfiguration:IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Title).IsRequired().HasMaxLength(100);
            builder.Property(c => c.ShortDescription).IsRequired().HasMaxLength(250);
            builder.Property(c => c.Description).IsRequired();
            builder.Property(c => c.ThumbnailUrl).IsRequired(false);
            builder.Property(c => c.Price).HasColumnType("decimal(18,2)");

            builder.HasOne(c => c.Instructor).WithMany(i => i.Courses).HasForeignKey(c => c.InstructorId).OnDelete(DeleteBehavior.NoAction);
            
        }
    }
}
