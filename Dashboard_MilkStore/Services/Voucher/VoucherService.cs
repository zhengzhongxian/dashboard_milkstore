using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Voucher;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace Dashboard_MilkStore.Services.Voucher
{
    public class VoucherService : IVoucherService
    {
        private readonly CallAPI _callAPI;
        private readonly ILogger<VoucherService> _logger;
        private readonly string _baseUrl;

        public VoucherService(CallAPI callAPI, IConfiguration configuration, ILogger<VoucherService> logger)
        {
            _callAPI = callAPI;
            _logger = logger;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://milkstore-grbpfnduezbpgvgc.eastasia-01.azurewebsites.net";
        }

        /// <summary>
        /// Lấy danh sách voucher có phân trang và lọc
        /// </summary>
        public async Task<ServiceResponse<PaginatedResult<VoucherViewModel>>> GetVouchersAsync(VoucherQueryViewModel query, string token)
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

                // Xử lý đặc biệt cho IsActive
                if (query.IsActive.HasValue)
                {
                    // Đảm bảo giá trị boolean được chuyển đổi đúng cách
                    string isActiveValue = query.IsActive.Value ? "true" : "false";
                    queryParams.Add($"IsActive={isActiveValue}");
                    _logger.LogInformation($"Adding IsActive filter: {isActiveValue}");
                }

                if (query.DiscountType.HasValue)
                {
                    queryParams.Add($"DiscountType={query.DiscountType.Value}");
                    _logger.LogInformation($"Adding DiscountType filter: {query.DiscountType.Value}");
                }

                if (query.IsRoot.HasValue)
                {
                    // Đảm bảo giá trị boolean được chuyển đổi đúng cách
                    string isRootValue = query.IsRoot.Value ? "true" : "false";
                    queryParams.Add($"IsRoot={isRootValue}");
                    _logger.LogInformation($"Adding IsRoot filter: {isRootValue}");
                }

                if (query.StartDateFrom.HasValue)
                {
                    queryParams.Add($"StartDateFrom={query.StartDateFrom.Value:yyyy-MM-dd}");
                }

                if (query.StartDateTo.HasValue)
                {
                    queryParams.Add($"StartDateTo={query.StartDateTo.Value:yyyy-MM-dd}");
                }

                if (query.EndDateFrom.HasValue)
                {
                    queryParams.Add($"EndDateFrom={query.EndDateFrom.Value:yyyy-MM-dd}");
                }

                if (query.EndDateTo.HasValue)
                {
                    queryParams.Add($"EndDateTo={query.EndDateTo.Value:yyyy-MM-dd}");
                }

                if (!string.IsNullOrEmpty(query.SortBy))
                {
                    queryParams.Add($"SortBy={HttpUtility.UrlEncode(query.SortBy)}");
                    queryParams.Add($"SortAscending={query.SortAscending.ToString().ToLower()}");
                }

                var queryString = string.Join("&", queryParams);
                var url = $"{_baseUrl}/api/Voucher/admin-staff?{queryString}";

                var response = await _callAPI.GetAsync<ServiceResponse<PaginatedResult<VoucherViewModel>>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<PaginatedResult<VoucherViewModel>>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<PaginatedResult<VoucherViewModel>>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Voucher API");
                return new ServiceResponse<PaginatedResult<VoucherViewModel>>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetVouchersAsync");
                return new ServiceResponse<PaginatedResult<VoucherViewModel>>().FailResponse("Đã xảy ra lỗi khi lấy danh sách voucher.");
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết của voucher
        /// </summary>
        public async Task<ServiceResponse<VoucherViewModel>> GetVoucherDetailAsync(string voucherId, string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/Voucher/{voucherId}";
                var response = await _callAPI.GetAsync<ServiceResponse<VoucherViewModel>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<VoucherViewModel>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<VoucherViewModel>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Voucher Detail API");
                return new ServiceResponse<VoucherViewModel>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetVoucherDetailAsync");
                return new ServiceResponse<VoucherViewModel>().FailResponse("Đã xảy ra lỗi khi lấy thông tin chi tiết voucher.");
            }
        }

        /// <summary>
        /// Tạo voucher mới
        /// </summary>
        public async Task<ServiceResponse<VoucherViewModel>> CreateVoucherAsync(CreateVoucherViewModel model, string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/Voucher";
                var response = await _callAPI.PostAsync<ServiceResponse<VoucherViewModel>>(url, model, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<VoucherViewModel>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<VoucherViewModel>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Create Voucher API");
                return new ServiceResponse<VoucherViewModel>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateVoucherAsync");
                return new ServiceResponse<VoucherViewModel>().FailResponse("Đã xảy ra lỗi khi tạo voucher mới.");
            }
        }

        /// <summary>
        /// Cập nhật thông tin voucher
        /// </summary>
        public async Task<ServiceResponse<VoucherViewModel>> UpdateVoucherAsync(UpdateVoucherViewModel model, string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/Voucher/{model.Voucherid}";

                // Tạo danh sách các thao tác PATCH
                var operations = new List<Dictionary<string, object>>();

                // Thêm các thuộc tính cần cập nhật
                operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/code" }, { "value", model.Code } });
                operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/discountValue" }, { "value", model.DiscountValue } });
                operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/discountType" }, { "value", model.DiscountType } });
                operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/startDate" }, { "value", model.StartDate } });
                operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/endDate" }, { "value", model.EndDate } });
                operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/usageLimit" }, { "value", model.UsageLimit } });
                operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/minOrder" }, { "value", model.MinOrder } });
                operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/isRoot" }, { "value", model.IsRoot } });

                if (model.MaxDiscount.HasValue)
                {
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/maxDiscount" }, { "value", model.MaxDiscount } });
                }

                if (model.Point.HasValue)
                {
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/point" }, { "value", model.Point } });
                }

                operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/isActive" }, { "value", model.IsActive } });

                if (!string.IsNullOrEmpty(model.Metadata))
                {
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/metadata" }, { "value", model.Metadata } });
                }

                _logger.LogInformation($"Sending PATCH request to: {url}");
                _logger.LogInformation($"PATCH operations: {System.Text.Json.JsonSerializer.Serialize(operations)}");

                var response = await _callAPI.PatchAsync<ServiceResponse<VoucherViewModel>>(url, operations, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<VoucherViewModel>().SuccessResponse(response.Data);
                }
                else
                {
                    return new ServiceResponse<VoucherViewModel>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Update Voucher API");
                return new ServiceResponse<VoucherViewModel>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateVoucherAsync");
                return new ServiceResponse<VoucherViewModel>().FailResponse("Đã xảy ra lỗi khi cập nhật thông tin voucher.");
            }
        }

        /// <summary>
        /// Xóa voucher
        /// </summary>
        public async Task<ServiceResponse<bool>> DeleteVoucherAsync(string voucherId, string token)
        {
            try
            {
                var url = $"{_baseUrl}/api/Voucher/{voucherId}";
                var response = await _callAPI.DeleteAsync<ServiceResponse<bool>>(url, token);

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
                _logger.LogError(ex, "Error calling Delete Voucher API");
                return new ServiceResponse<bool>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteVoucherAsync");
                return new ServiceResponse<bool>().FailResponse("Đã xảy ra lỗi khi xóa voucher.");
            }
        }

        /// <summary>
        /// Lấy danh sách voucher của khách hàng từ metadata
        /// </summary>
        public async Task<ServiceResponse<PaginatedResult<CustomerVoucherViewModel>>> GetCustomerVouchersAsync(VoucherQueryViewModel query, string token)
        {
            try
            {
                // Chuẩn bị URL với các tham số truy vấn
                var queryParams = new List<string>();
                queryParams.Add($"customerId={Uri.EscapeDataString(query.CustomerId)}");
                queryParams.Add($"pageNumber={query.PageNumber}");
                queryParams.Add($"pageSize={query.PageSize}");

                if (!string.IsNullOrEmpty(query.SearchTerm))
                {
                    queryParams.Add($"searchTerm={HttpUtility.UrlEncode(query.SearchTerm)}");
                }

                if (query.IsActive.HasValue)
                {
                    queryParams.Add($"isActive={query.IsActive.Value.ToString().ToLower()}");
                }

                if (query.DiscountType.HasValue)
                {
                    queryParams.Add($"discountType={query.DiscountType.Value}");
                }

                if (query.StartDateFrom.HasValue)
                {
                    queryParams.Add($"startDateFrom={query.StartDateFrom.Value:yyyy-MM-dd}");
                }

                if (query.StartDateTo.HasValue)
                {
                    queryParams.Add($"startDateTo={query.StartDateTo.Value:yyyy-MM-dd}");
                }

                if (query.EndDateFrom.HasValue)
                {
                    queryParams.Add($"endDateFrom={query.EndDateFrom.Value:yyyy-MM-dd}");
                }

                if (query.EndDateTo.HasValue)
                {
                    queryParams.Add($"endDateTo={query.EndDateTo.Value:yyyy-MM-dd}");
                }

                if (!string.IsNullOrEmpty(query.SortBy))
                {
                    queryParams.Add($"sortBy={HttpUtility.UrlEncode(query.SortBy)}");
                    queryParams.Add($"sortAscending={query.SortAscending.ToString().ToLower()}");
                }

                // Tạo URL cuối cùng
                var queryString = string.Join("&", queryParams);
                var url = $"{_baseUrl}/api/Voucher/admin-staff/customer-vouchers?{queryString}";

                _logger.LogInformation($"Calling API: {url}");

                // Gọi API
                var response = await _callAPI.GetAsync<ServiceResponse<PaginatedResult<CustomerVoucherViewModel>>>(url, token);

                if (response != null && response.Success)
                {
                    return new ServiceResponse<PaginatedResult<CustomerVoucherViewModel>>().SuccessResponse(response.Data);
                }
                else
                {
                    _logger.LogWarning($"API returned error: {response?.Message}");
                    return new ServiceResponse<PaginatedResult<CustomerVoucherViewModel>>().FailResponse(response?.Message ?? "Không nhận được phản hồi từ máy chủ");
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error calling Customer Vouchers API");
                return new ServiceResponse<PaginatedResult<CustomerVoucherViewModel>>().FailResponse("Không thể kết nối đến máy chủ. Vui lòng thử lại sau.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCustomerVouchersAsync");
                return new ServiceResponse<PaginatedResult<CustomerVoucherViewModel>>().FailResponse("Đã xảy ra lỗi khi lấy danh sách voucher của khách hàng.");
            }
        }
    }
}
