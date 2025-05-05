using Dashboard_MilkStore.Models.Statistics;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Statistics
{
    public interface IStatisticsService
    {
        Task<MonthlyRevenueResponse> GetMonthlyRevenueForYearAsync(int year);
    }
}
