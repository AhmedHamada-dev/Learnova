using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learnova.Application.Command.Courses.CreateCourse
{
    public class CreateCourseCommand:IRequest<int>
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ShortDescription { get; set; } = null!;
        public decimal? Price { get; set; }
        public bool IsFree { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
