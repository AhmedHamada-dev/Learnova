using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class Certificate
    {
        public int Id { get; set; }
        public string StudentId { get; set; }

        public ApplicationUser Student { get; set; }
        public int CourseId { get; set; }
        public Course Course {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
