using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Statistics;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        private readonly string _baseUrl;
        private readonly CallAPI _callAPI;

        public StatisticsService(string baseUrl, IHttpContextAccessor httpContextAccessor)
        {
            _baseUrl = baseUrl;
            _callAPI = new CallAPI(httpContextAccessor);
        }

        public async Task<MonthlyRevenueResponse> GetMonthlyRevenueForYearAsync(int year)
        {
            try
            {
                var url = $"{_baseUrl}/api/Statistics/monthly-revenue?year={year}";
                Console.WriteLine($"Calling API: {url}");

                // Không cần truyền token, CallAPI sẽ tự động lấy từ session
                var response = await _callAPI.GetAsync<MonthlyRevenueResponse>(url);

                if (response == null)
                {
                    return new MonthlyRevenueResponse
                    {
                        Success = false,
                        Message = "Failed to get monthly revenue. No response from server.",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetMonthlyRevenueForYearAsync: {ex.Message}");
                return new MonthlyRevenueResponse
                {
                    Success = false,
                    Message = $"Error getting monthly revenue: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ProductSalesResponse> GetProductSalesForMonthAsync(int month, int year)
        {
            try
            {
                var url = $"{_baseUrl}/api/Statistics/product-sales/{year}/{month}";
                Console.WriteLine($"Calling API: {url}");

                // Không cần truyền token, CallAPI sẽ tự động lấy từ session
                var response = await _callAPI.GetAsync<ProductSalesResponse>(url);

                if (response == null)
                {
                    return new ProductSalesResponse
                    {
                        Success = false,
                        Message = "Failed to get product sales. No response from server."
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetProductSalesForMonthAsync: {ex.Message}");
                return new ProductSalesResponse
                {
                    Success = false,
                    Message = $"Error getting product sales: {ex.Message}"
                };
            }
        }
    }
}
