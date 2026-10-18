using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApplicationLogs
    {

        [Key]
        public long Id { get; set; }
        public string? Level { get; set; }
        public string? Message { get; set; }
        public string? ExceptionType { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }
        public string? TraceId { get; set; }
        public string? UserId { get; set; }
        public string? RequestMethod { get; set; }
        public string? RequestPath { get; set; }
        public string? StatusCode { get; set; }
        public string? Source { get; set; }
        public string? Properties { get; set; }
        public int? EventId { get; set; }
    }
}
