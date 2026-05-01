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
    public class CertificateConfiguration:IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.HasKey(c => c.Id);
          
            builder.Property(c => c.CreatedAt).IsRequired();

            builder.HasOne(c => c.Student).WithMany(i => i.Certificates).HasForeignKey(c => c.StudentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(c => c.Course).WithMany().HasForeignKey(c => c.CourseId).OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => new { c.StudentId, c.CourseId }).IsUnique();



        }
    }
}
