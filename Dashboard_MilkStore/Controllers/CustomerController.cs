using Dashboard_MilkStore.Models.Customer;
using Dashboard_MilkStore.Services.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(
            ICustomerService customerService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool sortAscending = false,
            string statusId = null,
            string searchTerm = null,
            bool? isActive = null)
        {
            try
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                var token = HttpContext.Session.GetString("Token");
                if (token == null)
                {
                    _logger.LogInformation("Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập");
                    return RedirectToAction("Login", "Account");
                }

                // Tạo query model
                var query = new UserQueryViewModel
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortAscending = sortAscending,
                    StatusId = statusId,
                    SearchTerm = searchTerm,
                    IsActive = isActive
                };

                // Gọi service để lấy danh sách người dùng
                var response = await _customerService.GetUsersAsync(query, token);

                if (!response.Success)
                {
                    _logger.LogWarning("Lỗi khi lấy danh sách người dùng: {Message}", response.Message);
                    TempData["ErrorMessage"] = response.Message;
                    return View(new UserResponse());
                }

                // Truyền dữ liệu vào view
                var viewModel = new UserResponse
                {
                    Items = response.Data.Items,
                    Metadata = new UserMetadata
                    {
                        PageNumber = response.Data.Metadata.PageNumber,
                        PageSize = response.Data.Metadata.PageSize,
                        TotalCount = response.Data.Metadata.TotalCount,
                        TotalPages = response.Data.Metadata.TotalPages,
                        HasNext = response.Data.Metadata.HasNext,
                        HasPrevious = response.Data.Metadata.HasPrevious,
                        FirstItemOnPage = response.Data.Metadata.FirstItemOnPage
                    }
                };

                // Lưu các tham số tìm kiếm vào ViewBag để sử dụng trong view
                ViewBag.CurrentSort = sortBy;
                ViewBag.CurrentSortDirection = sortAscending;
                ViewBag.CurrentSearchTerm = searchTerm;
                ViewBag.CurrentStatusId = statusId;
                ViewBag.CurrentIsActive = isActive;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi lấy danh sách người dùng");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi lấy danh sách người dùng. Vui lòng thử lại sau.";
                return View(new UserResponse());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Lấy thông tin chi tiết của khách hàng
                var customerResponse = await _customerService.GetCustomerDetailAsync(id, token);
                if (!customerResponse.Success || customerResponse.Data == null)
                {
                    TempData["ErrorMessage"] = customerResponse.Message ?? "Không thể lấy thông tin khách hàng";
                    return RedirectToAction(nameof(Index));
                }

                // Lấy danh sách vai trò
                var rolesResponse = await _customerService.GetRolesAsync(token);
                if (rolesResponse.Success && rolesResponse.Data != null)
                {
                    customerResponse.Data.Roles = rolesResponse.Data;
                }

                // Lấy danh sách trạng thái người dùng
                var statusesResponse = await _customerService.GetUserStatusesAsync(token);
                if (statusesResponse.Success && statusesResponse.Data != null)
                {
                    customerResponse.Data.UserStatuses = statusesResponse.Data;
                }

                return View(customerResponse.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Details action");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi lấy thông tin khách hàng";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Details(string id, CustomerDetailViewModel model)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                // Xử lý ảnh base64 nếu có
                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file != null && file.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        await file.CopyToAsync(ms);
                        var fileBytes = ms.ToArray();
                        model.AvatarBase64 = Convert.ToBase64String(fileBytes);
                    }
                }
                else
                {
                    // Nếu không có ảnh mới, đặt AvatarBase64 thành chuỗi rỗng thay vì null
                    // để API biết rằng không cần cập nhật ảnh
                    model.AvatarBase64 = string.Empty;
                }

                // Xử lý checkbox IsActive
                model.IsActive = Request.Form.Keys.Contains("IsActive");

                // Cập nhật thông tin khách hàng
                var response = await _customerService.UpdateCustomerDetailAsync(id, model, token);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công";
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message ?? "Không thể cập nhật thông tin khách hàng";
                }

                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Details POST action");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật thông tin khách hàng";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
