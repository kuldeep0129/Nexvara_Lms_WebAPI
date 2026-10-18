using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Core.Dtos.Common
{
    public class ResponseDto <T>
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }
        public bool Success { get; set; }
        public int ResponseCode { get; set; }
        public string Message { get; set; }
    }
}
