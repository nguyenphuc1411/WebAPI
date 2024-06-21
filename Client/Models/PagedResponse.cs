using PagedList;

namespace Client.Models
{
    public class PagedResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; } 
        public int TotalPages { get; set; }
        public int TotalProducts { get; set; }
        public IPagedList<T> Data { get; set; }
    }
}
