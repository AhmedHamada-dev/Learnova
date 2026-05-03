using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class Exam
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string InstructorId { get; set; }
        public ApplicationUser Instructor { get; set; }

        public ICollection<Question> Questions { get; set; }
        public ICollection<StudentExam> StudentExams { get; set; }
    }
}
