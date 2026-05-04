using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }

        public decimal? Price { get; set; }
        public bool IsFree { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? ThumbnailUrl { get; set; }
        public string InstructorId { get; set; }
        public ApplicationUser Instructor { get; set; }

        public ICollection<Category> Categories { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Exam> Exams { get; set; }
    }
}
