using System;

namespace Dashboard_MilkStore.Models.Statistics
{
    public class CountResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int Data { get; set; }
        public int StatusCode { get; set; }
    }

    public class RevenueResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public decimal Data { get; set; }
        public int StatusCode { get; set; }
    }

    public class DashboardStats
    {
        public int OnlineCustomersCount { get; set; }
        public decimal TodayRevenue { get; set; }
        public int TodaySoldProductsCount { get; set; }
        public int PendingOrdersCount { get; set; }
    }
}
