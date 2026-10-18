using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum EntityStatus
    {
        [Description("Submitted")]
        Submitted =0,
        [Description("Issued")]
        Issued = 1,
        [Description("New")]
        New = 2,
    }
}
