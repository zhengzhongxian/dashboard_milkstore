using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Customer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Dashboard_MilkStore.Services.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly CallAPI _callAPI;
        private readonly ILogger<CustomerService> _logger;
        private readonly string _baseUrl;

        public CustomerService(CallAPI callAPI, IConfiguration configuration, ILogger<CustomerService> logger)
        {
            _callAPI = callAPI;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://milkstore-grbpfnduezbpgvgc.eastasia-01.azurewebsites.net";
        }

        public async Task<ServiceResponse<PaginatedResult<UserViewModel>>> GetUsersAsync(UserQueryViewModel query, string token)
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

                if (!string.IsNullOrEmpty(query.StatusId))
                {
                    queryParams.Add($"StatusId={HttpUtility.UrlEncode(query.StatusId)}");
                }

                if (query.IsActive.HasValue)
                {
                    queryParams.Add($"IsActive={query.IsActive.Value.ToString().ToLower()}");
                }

                if (!string.IsNullOrEmpty(query.SortBy))
                {
                    queryParams.Add($"SortBy={HttpUtility.UrlEncode(query.SortBy)}");
                    queryParams.Add($"SortAscending={query.SortAscending.ToString().ToLower()}");
                }

                var queryString = string.Join("&", queryParams);
                var url = $"{_baseUrl}/api/Customer/users?{queryString}";

                var response = await _callAPI.GetAsync<ServiceResponse<PaginatedResult<UserViewModel>>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<PaginatedResult<UserViewModel>>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<PaginatedResult<UserViewModel>>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Customer API");
                return new ServiceResponse<PaginatedResult<UserViewModel>>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUsersAsync");
                return new ServiceResponse<PaginatedResult<UserViewModel>>().FailResponse("Đã xảy ra lỗi khi lấy danh sách người dùng.");
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của khách hàng
        /// </summary>
        public async Task<ServiceResponse<CustomerDetailViewModel>> GetCustomerDetailAsync(string customerId, string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/Customer/{customerId}/detail";
                var response = await _callAPI.GetAsync<ServiceResponse<CustomerDetailViewModel>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<CustomerDetailViewModel>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<CustomerDetailViewModel>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Customer Detail API");
                return new ServiceResponse<CustomerDetailViewModel>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCustomerDetailAsync");
                return new ServiceResponse<CustomerDetailViewModel>().FailResponse("Đã xảy ra lỗi khi lấy thông tin chi tiết khách hàng.");
            }
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết của khách hàng
        /// </summary>
        public async Task<ServiceResponse<bool>> UpdateCustomerDetailAsync(string customerId, CustomerDetailViewModel viewModel, string token)
        {
            try
            {
                // Chuyển đổi từ ViewModel sang DTO
                var updateDto = new UpdateCustomerDetailDTO
                {
                    Username = viewModel.Username,
                    Email = viewModel.Email,
                    IsActive = viewModel.IsActive,
                    StatusId = viewModel.StatusId,
                    RoleId = viewModel.RoleId,
                    Surname = viewModel.Surname,
                    Middlename = viewModel.Middlename,
                    Firstname = viewModel.Firstname,
                    PhoneNumber = viewModel.PhoneNumber,
                    Address = viewModel.Address,
                    AvatarBase64 = viewModel.AvatarBase64,
                    Dob = viewModel.Dob,
                    Gender = viewModel.Gender,
                    Coupoun = viewModel.Coupoun
                };

                var url = $"{_baseUrl}/api/Customer/{customerId}/detail";
                var response = await _callAPI.PutAsync<ServiceResponse<bool>>(url, updateDto, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<bool>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<bool>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Customer Update API");
                return new ServiceResponse<bool>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCustomerDetailAsync");
                return new ServiceResponse<bool>().FailResponse("Đã xảy ra lỗi khi cập nhật thông tin chi tiết khách hàng.");
            }
        }

        /// <summary>
        /// Lấy danh sách các vai trò
        /// </summary>
        public async Task<ServiceResponse<List<RoleViewModel>>> GetRolesAsync(string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/Role";
                var response = await _callAPI.GetAsync<ServiceResponse<List<RoleViewModel>>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<List<RoleViewModel>>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<List<RoleViewModel>>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Role API");
                return new ServiceResponse<List<RoleViewModel>>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRolesAsync");
                return new ServiceResponse<List<RoleViewModel>>().FailResponse("Đã xảy ra lỗi khi lấy danh sách vai trò.");
            }
        }

        /// <summary>
        /// Lấy danh sách các trạng thái người dùng
        /// </summary>
        public async Task<ServiceResponse<List<UserStatusViewModel>>> GetUserStatusesAsync(string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/UserStatus";
                var response = await _callAPI.GetAsync<ServiceResponse<List<UserStatusViewModel>>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<List<UserStatusViewModel>>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<List<UserStatusViewModel>>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling UserStatus API");
                return new ServiceResponse<List<UserStatusViewModel>>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetUserStatusesAsync");
                return new ServiceResponse<List<UserStatusViewModel>>().FailResponse("Đã xảy ra lỗi khi lấy danh sách trạng thái người dùng.");
            }
        }
    }
}
