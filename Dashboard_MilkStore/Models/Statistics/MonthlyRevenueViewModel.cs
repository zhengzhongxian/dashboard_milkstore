using System.Collections.Generic;

namespace Dashboard_MilkStore.Models.Statistics
{
    public class MonthlyRevenueViewModel
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Revenue { get; set; }
    }

    public class YearlyRevenueViewModel
    {
        public int Year { get; set; }
        public List<MonthlyRevenueViewModel> MonthlyRevenues { get; set; } = new List<MonthlyRevenueViewModel>();
        public decimal TotalRevenue { get; set; }
    }

    public class MonthlyRevenueResponse
    {
        public YearlyRevenueViewModel Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
}
