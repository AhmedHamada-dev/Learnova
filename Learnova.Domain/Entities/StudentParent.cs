using Learnova.Domain.Identity;

namespace Learnova.Domain.Entities
{
    public class StudentParent
    {
        public int Id { get; set; }

        public string ParentId { get; set; } = null!;
        public ApplicationUser Parent { get; set; } = null!;

        public string StudentId { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;
    }
}
