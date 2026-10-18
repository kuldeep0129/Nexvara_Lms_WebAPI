using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.common
{
    public static class LogEvents
    {
        public static readonly EventId UnhandledException = new(1001,nameof(UnhandledException));
        public static readonly EventId DatabaseError = new(1002,nameof(DatabaseError));
        public static readonly EventId ValidationError = new(1003,nameof(ValidationError));
    }
}
