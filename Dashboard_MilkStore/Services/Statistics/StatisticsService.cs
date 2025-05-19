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

        public async Task<CountResponse> GetOnlineCustomersCountAsync()
        {
            try
            {
                var url = $"{_baseUrl}/api/Statistics/online-customers-count";
                Console.WriteLine($"Calling API: {url}");

                var response = await _callAPI.GetAsync<CountResponse>(url);

                if (response == null)
                {
                    return new CountResponse
                    {
                        Success = false,
                        Message = "Failed to get online customers count. No response from server.",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetOnlineCustomersCountAsync: {ex.Message}");
                return new CountResponse
                {
                    Success = false,
                    Message = $"Error getting online customers count: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<RevenueResponse> GetTodayRevenueAsync()
        {
            try
            {
                var url = $"{_baseUrl}/api/Statistics/today-revenue";
                Console.WriteLine($"Calling API: {url}");

                var response = await _callAPI.GetAsync<RevenueResponse>(url);

                if (response == null)
                {
                    return new RevenueResponse
                    {
                        Success = false,
                        Message = "Failed to get today's revenue. No response from server.",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetTodayRevenueAsync: {ex.Message}");
                return new RevenueResponse
                {
                    Success = false,
                    Message = $"Error getting today's revenue: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<CountResponse> GetTodaySoldProductsCountAsync()
        {
            try
            {
                var url = $"{_baseUrl}/api/Statistics/today-sold-products-count";
                Console.WriteLine($"Calling API: {url}");

                var response = await _callAPI.GetAsync<CountResponse>(url);

                if (response == null)
                {
                    return new CountResponse
                    {
                        Success = false,
                        Message = "Failed to get today's sold products count. No response from server.",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetTodaySoldProductsCountAsync: {ex.Message}");
                return new CountResponse
                {
                    Success = false,
                    Message = $"Error getting today's sold products count: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<CountResponse> GetPendingOrdersCountAsync()
        {
            try
            {
                var url = $"{_baseUrl}/api/Statistics/pending-orders-count";
                Console.WriteLine($"Calling API: {url}");

                var response = await _callAPI.GetAsync<CountResponse>(url);

                if (response == null)
                {
                    return new CountResponse
                    {
                        Success = false,
                        Message = "Failed to get pending orders count. No response from server.",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetPendingOrdersCountAsync: {ex.Message}");
                return new CountResponse
                {
                    Success = false,
                    Message = $"Error getting pending orders count: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<DashboardStats> GetDashboardStatsAsync()
        {
            var stats = new DashboardStats();

            try
            {
                // Obtener todas las estadísticas en paralelo para mejorar el rendimiento
                var onlineCustomersTask = GetOnlineCustomersCountAsync();
                var todayRevenueTask = GetTodayRevenueAsync();
                var todaySoldProductsTask = GetTodaySoldProductsCountAsync();
                var pendingOrdersTask = GetPendingOrdersCountAsync();

                // Esperar a que todas las tareas se completen
                await Task.WhenAll(onlineCustomersTask, todayRevenueTask, todaySoldProductsTask, pendingOrdersTask);

                // Asignar los resultados
                stats.OnlineCustomersCount = onlineCustomersTask.Result.Data;
                stats.TodayRevenue = todayRevenueTask.Result.Data;
                stats.TodaySoldProductsCount = todaySoldProductsTask.Result.Data;
                stats.PendingOrdersCount = pendingOrdersTask.Result.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetDashboardStatsAsync: {ex.Message}");
                // En caso de error, devolver valores predeterminados
                stats.OnlineCustomersCount = 0;
                stats.TodayRevenue = 0;
                stats.TodaySoldProductsCount = 0;
                stats.PendingOrdersCount = 0;
            }

            return stats;
        }
    }
}
