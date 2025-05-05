using System;
using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Order
{
    public class OrderQueryViewModel
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;
        
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
        
        public string CustomerId { get; set; }
        public string StatusId { get; set; }
        public string SearchTerm { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortBy { get; set; } = "OrderDate";
        public bool SortAscending { get; set; } = false;
    }
}
