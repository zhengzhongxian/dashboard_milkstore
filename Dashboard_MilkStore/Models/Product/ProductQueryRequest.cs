using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Product
{
    public class ProductQueryRequest
    {
        [Range(1, int.MaxValue)]
        [Required]
        public int PageNumber { get; set; }

        [Range(1, 20)]
        [Required(ErrorMessage = "PageSize không được vượt 20")]
        public int PageSize { get; set; }

        public string? CategoryId { get; set; }
        public string? TrendId { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "ProductName";
        public bool SortAscending { get; set; }
    }
} 