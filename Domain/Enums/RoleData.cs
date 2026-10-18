using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum RoleData
    {
        [Description("SuperAdmin")]
        SuperAdmin = 0,
        [Description("Admin")]
        Admin = 1,
        [Description("Student")]
        Student = 2,
        [Description("Trainer")]
        Trainer = 3,
        [Description("CorporatePartner")]
        CorporatePartner = 4,
    }

    public enum RoleStatus
    {
        [Description("Student")]
        Student = 2,
        [Description("Trainer")]
        Trainer = 3,
    }
}
