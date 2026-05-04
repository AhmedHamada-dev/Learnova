using Learnova.Application.Command.Courses.CreateCourse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.IServices
{
    public interface ICourseService
    {

        Task<int> CreateCourseAsync(string instructorId, CreateCourseCommand command, CancellationToken cancellationToken);
    }
}
