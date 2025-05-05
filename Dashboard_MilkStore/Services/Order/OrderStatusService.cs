using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Order
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly CallAPI _callAPI;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public OrderStatusService(CallAPI callAPI, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _callAPI = callAPI;
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5000";
        }

        public async Task<ServiceResponse<List<OrderStatusViewModel>>> GetAllOrderStatusesAsync()
        {
            try
            {
                var url = $"{_baseUrl}/api/OrderStatus";
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                
                Console.WriteLine($"Calling API: {url} + ");
                
                var response = await _callAPI.GetAsync<ServiceResponse<List<OrderStatusViewModel>>>(url, token);
                
                if (response == null)
                {
                    return new ServiceResponse<List<OrderStatusViewModel>>
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
                Console.WriteLine($"Error in GetAllOrderStatusesAsync: {ex.Message}");
                return new ServiceResponse<List<OrderStatusViewModel>>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
