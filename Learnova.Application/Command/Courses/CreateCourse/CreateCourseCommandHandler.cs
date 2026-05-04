using Learnova.Application.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Courses.CreateCourse
{
    public class CreateCourseCommandHandler
    {
        private readonly ICourseService _courseService;
        private readonly HttpContextAccessor _httpContextAccessor;

        public CreateCourseCommandHandler(ICourseService courseService, HttpContextAccessor httpContextAccessor)
        {
            _courseService = courseService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {

            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("No HttpContext available.");

            var instructorId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(instructorId))
                throw new UnauthorizedAccessException("InstructorId not found in token.");

            return await _courseService.CreateCourseAsync(instructorId, request, cancellationToken);
        }
    }
}
