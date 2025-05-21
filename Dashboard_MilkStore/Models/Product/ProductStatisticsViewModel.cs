using System;
using System.Collections.Generic;

namespace Dashboard_MilkStore.Models.Product
{
    public class ProductStatisticsViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalQuantity { get; set; }
        public List<MonthlyRevenueViewModel> MonthlyRevenues { get; set; } = new List<MonthlyRevenueViewModel>();
    }

    public class MonthlyRevenueViewModel
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
        public int Quantity { get; set; }
    }
}
