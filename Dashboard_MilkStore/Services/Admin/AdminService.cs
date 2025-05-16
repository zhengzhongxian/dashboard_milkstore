﻿using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Admin;
using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Customer;
using System.Web;

namespace Dashboard_MilkStore.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly CallAPI _callAPI;
        private readonly ILogger<AdminService> _logger;
        private readonly string _baseUrl;

        public AdminService(CallAPI callAPI, IConfiguration configuration, ILogger<AdminService> logger)
        {
            _callAPI = callAPI;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://milkstore-grbpfnduezbpgvgc.eastasia-01.azurewebsites.net";
        }

        public async Task<ServiceResponse<PaginatedResult<AdminViewModel>>> GetAdminStaffUsersAsync(AdminQueryViewModel query, string token)
        {
            try
            {
                var queryParams = new List<string>
                {
                    $"PageNumber={query.PageNumber}",
                    $"PageSize={query.PageSize}"
                };

                if (!string.IsNullOrEmpty(query.SearchTerm))
                {
                    queryParams.Add($"SearchTerm={HttpUtility.UrlEncode(query.SearchTerm)}");
                }

                if (!string.IsNullOrEmpty(query.RoleFilter))
                {
                    queryParams.Add($"RoleFilter={HttpUtility.UrlEncode(query.RoleFilter)}");
                }

                if (!string.IsNullOrEmpty(query.SortBy))
                {
                    queryParams.Add($"SortBy={HttpUtility.UrlEncode(query.SortBy)}");
                    queryParams.Add($"SortAscending={query.SortAscending.ToString().ToLower()}");
                }

                var queryString = string.Join("&", queryParams);
                var url = $"{_baseUrl}/api/Admin/users?{queryString}";

                var response = await _callAPI.GetAsync<ServiceResponse<PaginatedResult<AdminViewModel>>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<PaginatedResult<AdminViewModel>>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<PaginatedResult<AdminViewModel>>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách admin/staff");
                return new ServiceResponse<PaginatedResult<AdminViewModel>>().ErrorResponse($"Lỗi khi lấy danh sách admin/staff: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<AdminViewModel>> GetAdminStaffDetailAsync(string userId, string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/Admin/users/{userId}";
                var response = await _callAPI.GetAsync<ServiceResponse<AdminViewModel>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<AdminViewModel>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<AdminViewModel>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin chi tiết admin/staff");
                return new ServiceResponse<AdminViewModel>().ErrorResponse($"Lỗi khi lấy thông tin chi tiết admin/staff: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> UpdateAdminStaffAsync(string userId, UpdateAdminViewModel model, string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/Admin/users/{userId}";
                var response = await _callAPI.PutAsync<ServiceResponse<string>>(url, model, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<string>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<string>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin admin/staff");
                return new ServiceResponse<string>().ErrorResponse($"Lỗi khi cập nhật thông tin admin/staff: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<string>> UpdateCustomerFullAsync(string customerId, UpdateCustomerAdminViewModel model, string token)
        {
            try
            {
                // Xử lý ảnh base64 nếu có
                if (!string.IsNullOrEmpty(model.AvatarBase64))
                {
                    // Ảnh đã được xử lý ở controller, gửi lên API
                }
                else
                {
                    // Nếu không có ảnh mới, đặt AvatarBase64 thành null
                    model.AvatarBase64 = null;
                }

                var url = $"{_baseUrl}/api/Admin/customers/{customerId}";
                var response = await _callAPI.PutAsync<ServiceResponse<string>>(url, model, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<string>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<string>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin khách hàng");
                return new ServiceResponse<string>().ErrorResponse($"Lỗi khi cập nhật thông tin khách hàng: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<Models.Admin.RoleViewModel>>> GetRolesAsync(string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/Role";
                var response = await _callAPI.GetAsync<ServiceResponse<List<Models.Admin.RoleViewModel>>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<List<Models.Admin.RoleViewModel>>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<List<Models.Admin.RoleViewModel>>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách vai trò");
                return new ServiceResponse<List<Models.Admin.RoleViewModel>>().ErrorResponse($"Lỗi khi lấy danh sách vai trò: {ex.Message}");
            }
        }

        public async Task<ServiceResponse<List<Models.Admin.UserStatusViewModel>>> GetUserStatusesAsync(string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/UserStatus";
                var response = await _callAPI.GetAsync<ServiceResponse<List<Models.Admin.UserStatusViewModel>>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<List<Models.Admin.UserStatusViewModel>>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<List<Models.Admin.UserStatusViewModel>>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách trạng thái người dùng");
                return new ServiceResponse<List<Models.Admin.UserStatusViewModel>>().ErrorResponse($"Lỗi khi lấy danh sách trạng thái người dùng: {ex.Message}");
            }
        }
    }
}
