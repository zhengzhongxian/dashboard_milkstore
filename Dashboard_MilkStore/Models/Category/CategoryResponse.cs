using Dashboard_MilkStore.Models.Common;

namespace Dashboard_MilkStore.Models.Category
{
    public class CategoryResponse
    {
        public List<Category> Items { get; set; } = new List<Category>();
        public PaginationMetadata? Metadata { get; set; }
    }
}
