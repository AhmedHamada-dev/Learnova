using Learnova.Domain.Entities;
using Learnova.Domain.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Persistence
{
    public class LearnovaDbContext : IdentityDbContext<ApplicationUser>
    {
        public LearnovaDbContext(DbContextOptions<LearnovaDbContext> options)
            : base(options)
        {
        }
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<EmailVerification> EmailVerifications => Set<EmailVerification>();
        public DbSet<Coupon> Coupons => Set<Coupon>();
        public DbSet<LessonComment> LessonComments => Set<LessonComment>();
        public DbSet<Certificate> Certificates => Set<Certificate>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<Option> Options => Set<Option>();
        public DbSet<Question> Questions => Set<Question>();
        public DbSet<StudentExam> StudentExams => Set<StudentExam>();
        public DbSet<StudentParent> StudentParents => Set<StudentParent>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LearnovaDbContext).Assembly);
        }
    }
}
