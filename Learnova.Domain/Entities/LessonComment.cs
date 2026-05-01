using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class LessonComment
    {
        public int  Id { get; set; }
        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }
        public string Comment { get; set; }
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
