using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class Enrollment
    {
        public int Id { get; set; }
        public String StudentId { get; set; }
        public ApplicationUser Student { get; set; }
        public int CourseId { get; set; }

        public Course Course { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public int Progress { get; set; } = 0;
    }
}
