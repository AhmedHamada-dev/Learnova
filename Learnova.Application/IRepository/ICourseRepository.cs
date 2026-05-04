using Learnova.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.IRepository
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<IReadOnlyList<Course>> GetByInstructorAsync(string instructorId, CancellationToken cancellationToken = default);
    }
}
