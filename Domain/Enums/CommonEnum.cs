using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum CommonEnum
    {
  

    }
    public enum GenderEnum
    {
        [Description("Male")]
        Male = 1,
        [Description("Female")]
        Female = 2,
        [Description("Other")]
        Other = 3,

    }
    public enum CourseAccessType
    {
        [Description("Free")]
        Free = 1,
        [Description("Paid")]
        Paid = 2
    }
    public enum BatchStatus
    {
        Draft = 1,
        Upcoming = 2,
        Active = 3,
        Completed = 4,
        Cancelled = 5
    }
    public enum ScheduleClassType
    {
        Live = 1,
        Offline = 2,
        Recorded =3
    }
    public enum AdminTeacherAssingStatus
    {
        All = 1,
        Active = 2,
        Upcomming =3,
        Draft =4,
        Completed =5
    }
}
