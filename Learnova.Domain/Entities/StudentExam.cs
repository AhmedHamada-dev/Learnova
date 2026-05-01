using Learnova.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Domain.Entities
{
    public class StudentExam
    {
        public int Id { get; set; }
        public string StudentId { get; set; }
        public ApplicationUser Student { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public double Score { get; set; }
        public DateTime TakenAt { get; set; } = DateTime.UtcNow;
    }
}
