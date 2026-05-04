using Learnova.Application.IRepository;
using Learnova.Domain.Entities;
using Learnova.Infrastructure.Persistence.Repositories;
using Learnova.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Infrastructure.Repository
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(LearnovaDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Course>> GetByInstructorAsync(string instructorId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.InstructorId == instructorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        // ممكن override GetByIdAsync لو عايز Includeات معينة:
        public override async Task<Course?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.Categories)
                    .ThenInclude(cat => cat.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
    }
}
