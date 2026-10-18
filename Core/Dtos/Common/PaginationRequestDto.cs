using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Common
{
    public class PaginationRequestDto
    {
        private const int MaxPageSize = 100;
        public int PageNumber { get; set; } = 1;
        private int _PageSize = 10;
        public int PageSize { 
            get => _PageSize;
            set => _PageSize = value > MaxPageSize ? MaxPageSize : value;} 
    }
}
