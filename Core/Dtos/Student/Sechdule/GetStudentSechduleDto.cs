using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Student.Sechdule
{
    public class GetStudentSechduleDto
    {
        public int Id { get; set; }
        public DateTime Date {get; set;}
        public TimeSpan Time { get; set;}
        public string BatchName { get; set;}
        public string ModuleName { get; set; }
    }
   
}
