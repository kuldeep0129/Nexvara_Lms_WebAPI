using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Common
{
    public class FileUploadResultDto
    {
        public string PublicId { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}
