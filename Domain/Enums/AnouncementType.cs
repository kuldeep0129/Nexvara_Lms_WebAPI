using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum AnouncementType
    {
        [Description("Admission")]
        Admission=0,
        [Description("Course")]
        Course = 1,
        [Description("Event")]
        Event = 2,
        [Description("Workshop")]
        Workshop = 3,
        [Description("General")]
        General = 4,
    
    }
}
