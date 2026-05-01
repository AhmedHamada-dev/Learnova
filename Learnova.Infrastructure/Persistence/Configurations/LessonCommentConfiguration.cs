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
    public class LessonCommentConfiguration:IEntityTypeConfiguration<LessonComment>
    {
        public void Configure(EntityTypeBuilder<LessonComment> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(c => c.Comment).IsRequired().HasMaxLength(500);
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.HasOne(c => c.Student).WithMany(s => s.LessonComments).HasForeignKey(c => c.StudentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(c => c.Lesson).WithMany(s => s.lessonComments).HasForeignKey(c => c.LessonId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
