using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos.Common
{
    public class PageResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
       public int TotalPages =>(int)Math.Ceiling((double)TotalCount / (double)PageSize);
        public bool HashPreviousPage => PageNumber > 1;
        public bool HashNextPage => PageNumber < TotalPages;
    }
}
