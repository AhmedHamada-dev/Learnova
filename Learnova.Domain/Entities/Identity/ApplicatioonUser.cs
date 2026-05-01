using Learnova.Domain.Entities;
using Learnova.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Identity
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public UserRole Role { get; set; } = UserRole.Student;
        public bool IsVerified { get; set; }

        public ICollection<LessonComment> LessonComments { get; set; }
        public ICollection<Certificate> Certificates { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Course> Courses { get; set; }
        public ICollection<EmailVerification> EmailVerifications { get; set; }

        public ICollection<Exam> CreatedExams { get; set; } // للـ Instructor
        public ICollection<StudentExam> TakenExams { get; set; } // للـ Student
    }
}
