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
    public class StudentExamConfiguration:IEntityTypeConfiguration<StudentExam>
    {
        public void Configure(EntityTypeBuilder<StudentExam> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Score).IsRequired();
            builder.Property(e => e.TakenAt).IsRequired();

            builder.HasOne(e => e.Exam)
                .WithMany(c=>c.StudentExams)
                .HasForeignKey(e => e.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Student)
                .WithMany(u => u.TakenExams)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
