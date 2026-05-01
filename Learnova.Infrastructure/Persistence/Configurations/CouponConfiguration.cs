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
    public class CouponConfiguration:IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Code).IsRequired().HasMaxLength(20);
            builder.Property(c => c.IsActive).HasDefaultValue(true);
            builder.Property(c => c.MakesFree).HasDefaultValue(false);
            builder.HasIndex(c => c.Code).IsUnique();
            builder.Property(c => c.DiscountPercentage).HasColumnType("decimal(5,2)");

            builder.HasOne(c => c.Course).WithMany().HasForeignKey(c => c.CourseId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
