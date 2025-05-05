using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly CallAPI _callAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public OrderService(CallAPI callAPI, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _callAPI = callAPI;
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5000";
        }

        public async Task<List<OrderViewModel>> GetAllOrdersAsync()
        {
            try
            {
                var url = $"{_baseUrl}/api/Order";
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var response = await _callAPI.GetAsync<ServiceResponse<List<OrderViewModel>>>(url, token);
                
                if (response != null && response.Success)
                {
                    return response.Data ?? new List<OrderViewModel>();
                }
                
                return new List<OrderViewModel>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllOrdersAsync: {ex.Message}");
                return new List<OrderViewModel>();
            }
        }

        public async Task<OrderViewModel> GetOrderByIdAsync(string id)
        {
            try
            {
                var url = $"{_baseUrl}/api/Order/{id}";
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var response = await _callAPI.GetAsync<ServiceResponse<OrderViewModel>>(url, token);
                
                if (response != null && response.Success)
                {
                    return response.Data;
                }
                
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetOrderByIdAsync: {ex.Message}");
                return null;
            }
        }

        public async Task<ServiceResponse<PaginatedResult<OrderViewModel>>> GetPaginatedOrdersAsync(OrderQueryViewModel query)
        {
            try
            {
                // Build query string
                var queryParams = new List<string>();
                queryParams.Add($"PageNumber={query.PageNumber}");
                queryParams.Add($"PageSize={query.PageSize}");
                
                if (!string.IsNullOrEmpty(query.CustomerId))
                    queryParams.Add($"CustomerId={Uri.EscapeDataString(query.CustomerId)}");
                
                if (!string.IsNullOrEmpty(query.StatusId))
                    queryParams.Add($"StatusId={Uri.EscapeDataString(query.StatusId)}");
                
                if (!string.IsNullOrEmpty(query.SearchTerm))
                    queryParams.Add($"SearchTerm={Uri.EscapeDataString(query.SearchTerm)}");
                
                if (query.StartDate.HasValue)
                    queryParams.Add($"StartDate={query.StartDate.Value:yyyy-MM-dd}");
                
                if (query.EndDate.HasValue)
                    queryParams.Add($"EndDate={query.EndDate.Value:yyyy-MM-dd}");
                
                if (!string.IsNullOrEmpty(query.SortBy))
                    queryParams.Add($"SortBy={Uri.EscapeDataString(query.SortBy)}");
                
                queryParams.Add($"SortAscending={query.SortAscending}");
                
                var queryString = string.Join("&", queryParams);
                var url = $"{_baseUrl}/api/Order/paginated?{queryString}";
                
                Console.WriteLine($"Calling API: {url}");
                
                // Lấy token từ session
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                
                // Gọi API với token
                var response = await _callAPI.GetAsync<ServiceResponse<PaginatedResult<OrderViewModel>>>(url, token);
                
                if (response == null)
                {
                    return new ServiceResponse<PaginatedResult<OrderViewModel>>
                    {
                        Success = false,
                        Message = "Không thể kết nối đến API",
                        StatusCode = 500
                    };
                }
                
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetPaginatedOrdersAsync: {ex.Message}");
                return new ServiceResponse<PaginatedResult<OrderViewModel>>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
