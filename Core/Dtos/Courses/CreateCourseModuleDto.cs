using Domain.Entities.UserInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Courses
{
    public class CreateCourseModuleDto
    {

        public int? CourseId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int Sortorder { get; set; }
    }
}
