﻿using Dashboard_MilkStore.Models.Admin;
using Dashboard_MilkStore.Models.Customer;
using Dashboard_MilkStore.Services.Admin;
using Dashboard_MilkStore.Services.Auth;
using Dashboard_MilkStore.Services.Customer;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Dashboard_MilkStore.Controllers
{
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly ICustomerService _customerService;
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            IAdminService adminService,
            ICustomerService customerService,
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _customerService = customerService;
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool sortAscending = false,
            string? roleFilter = null,
            string? searchTerm = null)
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
                var query = new AdminQueryViewModel
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortAscending = sortAscending,
                    RoleFilter = roleFilter,
                    SearchTerm = searchTerm
                };

                // Gọi API để lấy danh sách admin/staff
                var response = await _adminService.GetAdminStaffUsersAsync(query, token);
                if (!response.Success)
                {
                    TempData["ErrorMessage"] = response.Message;
                    return View(new Models.Common.PaginatedResult<AdminViewModel>());
                }

                // Lưu các tham số hiện tại vào ViewBag để sử dụng trong view
                ViewBag.CurrentPageNumber = pageNumber;
                ViewBag.CurrentPageSize = pageSize;
                ViewBag.CurrentSortBy = sortBy;
                ViewBag.CurrentSortAscending = sortAscending;
                ViewBag.CurrentRoleFilter = roleFilter;
                ViewBag.CurrentSearchTerm = searchTerm;

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách admin/staff");
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return View(new Models.Common.PaginatedResult<AdminViewModel>());
            }
        }

        public async Task<IActionResult> Edit(string id)
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
                // Lấy thông tin chi tiết của admin/staff
                var adminResponse = await _adminService.GetAdminStaffDetailAsync(id, token);
                if (!adminResponse.Success || adminResponse.Data == null)
                {
                    TempData["ErrorMessage"] = adminResponse.Message ?? "Không thể lấy thông tin người dùng";
                    return RedirectToAction(nameof(Index));
                }

                // Tạo view model
                var viewModel = new UpdateAdminViewModel
                {
                    UserId = adminResponse.Data.UserId,
                    Username = adminResponse.Data.Username,
                    Email = adminResponse.Data.Email,
                    IsActive = adminResponse.Data.IsActive,
                    StatusId = adminResponse.Data.StatusId,
                    RoleId = adminResponse.Data.RoleId
                };

                // Lấy danh sách vai trò
                var rolesResponse = await _adminService.GetRolesAsync(token);
                if (rolesResponse.Success && rolesResponse.Data != null)
                {
                    viewModel.Roles = rolesResponse.Data;
                }

                // Lấy danh sách trạng thái người dùng
                var statusesResponse = await _adminService.GetUserStatusesAsync(token);
                if (statusesResponse.Success && statusesResponse.Data != null)
                {
                    viewModel.UserStatuses = statusesResponse.Data;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin chi tiết admin/staff");
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, UpdateAdminViewModel model)
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
                // Xử lý checkbox IsActive
                model.IsActive = Request.Form.Keys.Contains("IsActive");

                // Cập nhật thông tin admin/staff
                var response = await _adminService.UpdateAdminStaffAsync(id, model, token);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin người dùng thành công";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message ?? "Không thể cập nhật thông tin người dùng";

                    // Lấy lại danh sách vai trò và trạng thái
                    var rolesResponse = await _adminService.GetRolesAsync(token);
                    if (rolesResponse.Success && rolesResponse.Data != null)
                    {
                        model.Roles = rolesResponse.Data;
                    }

                    var statusesResponse = await _adminService.GetUserStatusesAsync(token);
                    if (statusesResponse.Success && statusesResponse.Data != null)
                    {
                        model.UserStatuses = statusesResponse.Data;
                    }

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin admin/staff");
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";

                // Lấy lại danh sách vai trò và trạng thái
                var rolesResponse = await _adminService.GetRolesAsync(token);
                if (rolesResponse.Success && rolesResponse.Data != null)
                {
                    model.Roles = rolesResponse.Data;
                }

                var statusesResponse = await _adminService.GetUserStatusesAsync(token);
                if (statusesResponse.Success && statusesResponse.Data != null)
                {
                    model.UserStatuses = statusesResponse.Data;
                }

                return View(model);
            }
        }

        public async Task<IActionResult> EditCustomer(string id)
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
                // Kiểm tra vai trò của người dùng
                string role = _authService.GetRoleFromToken(token);
                bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

                // Lấy thông tin chi tiết của khách hàng
                var customerResponse = await _customerService.GetCustomerDetailAsync(id, token);
                if (!customerResponse.Success || customerResponse.Data == null)
                {
                    TempData["ErrorMessage"] = customerResponse.Message ?? "Không thể lấy thông tin khách hàng";
                    return RedirectToAction("Index", "Customer");
                }

                // Tạo view model
                var viewModel = new UpdateCustomerAdminViewModel
                {
                    CustomerId = customerResponse.Data.CustomerId,
                    Username = customerResponse.Data.Username,
                    Email = customerResponse.Data.Email,
                    Surname = customerResponse.Data.Surname,
                    Middlename = customerResponse.Data.Middlename,
                    Firstname = customerResponse.Data.Firstname,
                    FullName = customerResponse.Data.FullName,
                    PhoneNumber = customerResponse.Data.PhoneNumber,
                    Address = customerResponse.Data.Address,
                    Avatar = customerResponse.Data.Avatar,
                    Dob = customerResponse.Data.Dob,
                    Gender = customerResponse.Data.Gender,
                    Coupoun = customerResponse.Data.Coupoun,
                    IsActive = customerResponse.Data.IsActive,
                    StatusId = customerResponse.Data.StatusId,
                    CreatedAt = customerResponse.Data.CreatedAt,
                    UpdatedAt = customerResponse.Data.UpdatedAt
                };

                // Lấy danh sách trạng thái người dùng
                var statusesResponse = await _adminService.GetUserStatusesAsync(token);
                if (statusesResponse.Success && statusesResponse.Data != null)
                {
                    viewModel.UserStatuses = statusesResponse.Data;
                }

                // Truyền thông tin về vai trò người dùng vào ViewBag
                ViewBag.IsAdmin = isAdmin;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin chi tiết khách hàng");
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
                return RedirectToAction("Index", "Customer");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditCustomer(string id, UpdateCustomerAdminViewModel model)
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
                // Kiểm tra vai trò của người dùng
                string role = _authService.GetRoleFromToken(token);
                bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

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
                    model.AvatarBase64 = string.Empty;
                }

                // Xử lý checkbox IsActive
                model.IsActive = Request.Form.Keys.Contains("IsActive");

                // Cập nhật thông tin khách hàng
                var response = await _adminService.UpdateCustomerFullAsync(id, model, token);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Cập nhật thông tin khách hàng thành công";
                    return RedirectToAction("Details", "Customer", new { id });
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message ?? "Không thể cập nhật thông tin khách hàng";

                    // Lấy lại danh sách trạng thái
                    var statusesResponse = await _adminService.GetUserStatusesAsync(token);
                    if (statusesResponse.Success && statusesResponse.Data != null)
                    {
                        model.UserStatuses = statusesResponse.Data;
                    }

                    // Truyền thông tin về vai trò người dùng vào ViewBag
                    ViewBag.IsAdmin = isAdmin;

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật thông tin khách hàng");
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";

                // Lấy lại danh sách trạng thái
                var statusesResponse = await _adminService.GetUserStatusesAsync(token);
                if (statusesResponse.Success && statusesResponse.Data != null)
                {
                    model.UserStatuses = statusesResponse.Data;
                }

                // Truyền thông tin về vai trò người dùng vào ViewBag
                string role = _authService.GetRoleFromToken(token);
                bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);
                ViewBag.IsAdmin = isAdmin;

                return View(model);
            }
        }
    }
}
