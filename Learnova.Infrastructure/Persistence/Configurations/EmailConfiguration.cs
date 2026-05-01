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
    public class EmailConfiguration : IEntityTypeConfiguration<EmailVerification>
    {
        public void Configure(EntityTypeBuilder<EmailVerification> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).IsRequired().HasMaxLength(6);
            builder.Property(x => x.IsUsed).HasDefaultValue(false);
            builder.Property(x => x.ExpireAt).IsRequired();
            builder.HasOne(x => x.User).WithMany(c => c.EmailVerifications).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(v => v.Code);
        }
    }
}
