using Dashboard_MilkStore.Models.Statistics;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Statistics
{
    public interface IStatisticsService
    {
        Task<MonthlyRevenueResponse> GetMonthlyRevenueForYearAsync(int year);
        Task<ProductSalesResponse> GetProductSalesForMonthAsync(int month, int year);

        // Nuevos métodos para las estadísticas del dashboard
        Task<CountResponse> GetOnlineCustomersCountAsync();
        Task<RevenueResponse> GetTodayRevenueAsync();
        Task<CountResponse> GetTodaySoldProductsCountAsync();
        Task<CountResponse> GetPendingOrdersCountAsync();

        // Método para obtener todas las estadísticas en una sola llamada
        Task<DashboardStats> GetDashboardStatsAsync();
    }
}
