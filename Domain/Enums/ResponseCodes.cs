using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum APIResponseCodes
    {
        [Description("OK")]
        OK =200,
        [Description("Created")]
        Created =201,
        [Description("UnAuthrized")]
        UnAuthrized = 401,
        [Description("BadRequest")]
        BadRequest = 400,
        [Description("Forbidden")]
        Forbidden = 403,
        [Description("Internal Server Error")]
        InternalServerError = 500,
        [Description("NotFound")]
        NotFound = 404,
    }
}
