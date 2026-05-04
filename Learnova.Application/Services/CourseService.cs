using Learnova.Application.Command.Courses.CreateCourse;
using Learnova.Application.IRepository;
using Learnova.Application.IServices;
using Learnova.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Services
{
    public class CourseService:ICourseService
    {


        private readonly ICourseRepository _courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository;
        }


        public async Task<int> CreateCourseAsync(string instructorId, CreateCourseCommand command, CancellationToken cancellationToken)
        {
            var course = new Course
            {
                InstructorId = instructorId,
                Title = command.Title,
                Description = command.Description,
                Price = command.Price,
                IsFree = command.IsFree,
                ThumbnailUrl = command.ThumbnailUrl,
                IsPublished = false, // default: draft
                CreatedAt = DateTime.UtcNow
            };

            await _courseRepository.AddAsync(course, cancellationToken);

            return course.Id;
        }
    }
}
