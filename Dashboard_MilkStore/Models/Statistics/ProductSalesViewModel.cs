using System.Collections.Generic;

namespace Dashboard_MilkStore.Models.Statistics
{
    public class ProductSalesViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal TotalSales { get; set; }
    }

    public class MonthlySalesViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public List<ProductSalesViewModel> Products { get; set; } = new List<ProductSalesViewModel>();
        public decimal TotalSales { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class ProductSalesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public MonthlySalesViewModel Data { get; set; }
    }
}
