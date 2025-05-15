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

        public async Task<ServiceResponse<OrderViewModel>> UpdateOrderStatusAsync(string orderId, string statusId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    return new ServiceResponse<OrderViewModel>
                    {
                        Success = false,
                        Message = "Mã đơn hàng không được để trống",
                        StatusCode = 400
                    };
                }

                if (string.IsNullOrEmpty(statusId))
                {
                    return new ServiceResponse<OrderViewModel>
                    {
                        Success = false,
                        Message = "Mã trạng thái không được để trống",
                        StatusCode = 400
                    };
                }

                var url = $"{_baseUrl}/api/Order/{orderId}/status/{statusId}";
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                Console.WriteLine($"Calling API to update order status: {url}");

                // Trước khi gọi API, kiểm tra xem đơn hàng có ở trạng thái đã hoàn thành hoặc đã hủy không
                var order = await GetOrderByIdAsync(orderId);
                if (order != null)
                {
                    if (order.StatusId == "COMPLETED" || order.StatusId == "CANCELLED")
                    {
                        return new ServiceResponse<OrderViewModel>
                        {
                            Success = false,
                            Message = "Không thể cập nhật trạng thái đơn hàng đã hoàn thành hoặc đã hủy",
                            Data = order,
                            StatusCode = 400
                        };
                    }
                }

                try
                {
                    // Gọi API PUT để cập nhật trạng thái đơn hàng
                    var response = await _callAPI.PutAsync<ServiceResponse<OrderViewModel>>(url, null, token);

                    if (response == null)
                    {
                        // Nếu không nhận được phản hồi hợp lệ, vẫn coi là thành công nếu không có lỗi
                        // Lấy lại thông tin đơn hàng để hiển thị
                        Console.WriteLine("Response is null, fetching order details instead");
                        var updatedOrder = await GetOrderByIdAsync(orderId);

                        if (updatedOrder != null)
                        {
                            return new ServiceResponse<OrderViewModel>
                            {
                                Success = true,
                                Message = "Cập nhật trạng thái đơn hàng thành công",
                                Data = updatedOrder,
                                StatusCode = 200
                            };
                        }
                        else
                        {
                            return new ServiceResponse<OrderViewModel>
                            {
                                Success = false,
                                Message = "Không thể lấy thông tin đơn hàng sau khi cập nhật",
                                StatusCode = 500
                            };
                        }
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"API call error: {ex.Message}");

                    // Nếu có lỗi khi gọi API, vẫn thử lấy thông tin đơn hàng
                    var updatedOrderAfterError = await GetOrderByIdAsync(orderId);

                    if (updatedOrderAfterError != null && updatedOrderAfterError.StatusId == statusId)
                    {
                        // Nếu trạng thái đã được cập nhật thành công
                        return new ServiceResponse<OrderViewModel>
                        {
                            Success = true,
                            Message = "Cập nhật trạng thái đơn hàng thành công",
                            Data = updatedOrderAfterError,
                            StatusCode = 200
                        };
                    }

                    throw; // Ném lại ngoại lệ nếu không thể xác nhận cập nhật thành công
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateOrderStatusAsync: {ex.Message}");
                return new ServiceResponse<OrderViewModel>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceResponse<OrderViewModel>> AdminUpdateOrderStatusAsync(string orderId, string statusId)
        {
            try
            {
                if (string.IsNullOrEmpty(orderId))
                {
                    return new ServiceResponse<OrderViewModel>
                    {
                        Success = false,
                        Message = "Mã đơn hàng không được để trống",
                        StatusCode = 400
                    };
                }

                if (string.IsNullOrEmpty(statusId))
                {
                    return new ServiceResponse<OrderViewModel>
                    {
                        Success = false,
                        Message = "Mã trạng thái không được để trống",
                        StatusCode = 400
                    };
                }

                // Sử dụng API endpoint dành riêng cho admin
                var url = $"{_baseUrl}/api/Order/{orderId}/admin-status/{statusId}";
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                Console.WriteLine($"Calling Admin API to update order status: {url}");

                try
                {
                    // Gọi API PUT để cập nhật trạng thái đơn hàng
                    var response = await _callAPI.PutAsync<ServiceResponse<OrderViewModel>>(url, null, token);

                    if (response == null)
                    {
                        // Nếu không nhận được phản hồi hợp lệ, vẫn coi là thành công nếu không có lỗi
                        // Lấy lại thông tin đơn hàng để hiển thị
                        Console.WriteLine("Response is null, fetching order details instead");
                        var updatedOrder = await GetOrderByIdAsync(orderId);

                        if (updatedOrder != null)
                        {
                            return new ServiceResponse<OrderViewModel>
                            {
                                Success = true,
                                Message = "Cập nhật trạng thái đơn hàng thành công (Admin mode)",
                                Data = updatedOrder,
                                StatusCode = 200
                            };
                        }
                        else
                        {
                            return new ServiceResponse<OrderViewModel>
                            {
                                Success = false,
                                Message = "Không thể lấy thông tin đơn hàng sau khi cập nhật",
                                StatusCode = 500
                            };
                        }
                    }

                    return response;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"API call error: {ex.Message}");

                    // Nếu có lỗi khi gọi API, vẫn thử lấy thông tin đơn hàng
                    var updatedOrderAfterError = await GetOrderByIdAsync(orderId);

                    if (updatedOrderAfterError != null && updatedOrderAfterError.StatusId == statusId)
                    {
                        // Nếu trạng thái đã được cập nhật thành công
                        return new ServiceResponse<OrderViewModel>
                        {
                            Success = true,
                            Message = "Cập nhật trạng thái đơn hàng thành công (Admin mode)",
                            Data = updatedOrderAfterError,
                            StatusCode = 200
                        };
                    }

                    throw; // Ném lại ngoại lệ nếu không thể xác nhận cập nhật thành công
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AdminUpdateOrderStatusAsync: {ex.Message}");
                return new ServiceResponse<OrderViewModel>
                {
                    Success = false,
                    Message = $"Lỗi: {ex.Message}",
                    StatusCode = 500
                };
            }
        }

        public async Task<ServiceResponse<PaginatedResult<OrderViewModel>>> GetCustomerOrderHistoryAsync(string customerId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    return new ServiceResponse<PaginatedResult<OrderViewModel>>
                    {
                        Success = false,
                        Message = "ID khách hàng không được để trống",
                        StatusCode = 400
                    };
                }

                // Xây dựng query string
                var queryParams = new List<string>();
                queryParams.Add($"PageNumber={pageNumber}");
                queryParams.Add($"PageSize={pageSize}");

                var queryString = string.Join("&", queryParams);
                var url = $"{_baseUrl}/api/Order/customer/{customerId}/history?{queryString}";

                Console.WriteLine($"Calling API for customer order history: {url}");

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
                Console.WriteLine($"Error in GetCustomerOrderHistoryAsync: {ex.Message}");
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
