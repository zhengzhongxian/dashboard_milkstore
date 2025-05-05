using System.Collections.Generic;

namespace Dashboard_MilkStore.Models.Common
{
    public class PaginatedResult<T>
    {
        public PaginationMetadata Metadata { get; set; }
        public List<T> Items { get; set; } = new List<T>();
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }

        public PaginatedResult()
        {
            Metadata = new PaginationMetadata();
        }

        public PaginatedResult(List<T> items, PaginationMetadata metadata)
        {
            Items = items;
            Metadata = metadata;
        }
    }

    public class PaginationMetadata
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }
        public int TotalPages => (int)System.Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
        public int FirstItemOnPage => (PageNumber - 1) * PageSize + 1;
    }
}
