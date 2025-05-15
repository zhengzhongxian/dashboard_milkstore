using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Voucher;
using Dashboard_MilkStore.Services.Voucher;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Controllers
{
    public class VoucherController : Controller
    {
        private readonly IVoucherService _voucherService;
        private readonly ILogger<VoucherController> _logger;

        public VoucherController(IVoucherService voucherService, ILogger<VoucherController> logger)
        {
            _voucherService = voucherService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool sortAscending = false,
            string searchTerm = null,
            bool? isActive = null,
            int? discountType = null,
            bool? isRoot = null)
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
                var query = new VoucherQueryViewModel
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SortBy = sortBy,
                    SortAscending = sortAscending,
                    SearchTerm = searchTerm,
                    IsActive = isActive,
                    DiscountType = discountType,
                    IsRoot = isRoot
                };

                // Gọi service để lấy danh sách voucher
                var response = await _voucherService.GetVouchersAsync(query, token);

                if (!response.Success)
                {
                    _logger.LogWarning("Lỗi khi lấy danh sách voucher: {Message}", response.Message);
                    TempData["ErrorMessage"] = response.Message;
                    return View(new PaginatedResult<VoucherViewModel>());
                }

                // Lưu các tham số tìm kiếm vào ViewBag để sử dụng trong view
                ViewBag.CurrentSort = sortBy;
                ViewBag.CurrentSortDirection = sortAscending;
                ViewBag.CurrentSearchTerm = searchTerm;
                ViewBag.CurrentIsActive = isActive;
                ViewBag.CurrentDiscountType = discountType;
                ViewBag.CurrentIsRoot = isRoot;
                ViewBag.CurrentPageSize = pageSize;

                return View(response.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi lấy danh sách voucher");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi lấy danh sách voucher. Vui lòng thử lại sau.";
                return View(new PaginatedResult<VoucherViewModel>());
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new CreateVoucherViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(30),
                IsActive = true,
                IsRoot = true,
                UsageLimit = 100,
                MinOrder = 0
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateVoucherViewModel model)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _voucherService.CreateVoucherAsync(model, token);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Tạo voucher thành công";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo voucher mới");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tạo voucher mới. Vui lòng thử lại sau.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var response = await _voucherService.GetVoucherDetailAsync(id, token);
                if (!response.Success)
                {
                    TempData["ErrorMessage"] = response.Message;
                    return RedirectToAction(nameof(Index));
                }

                var voucher = response.Data;
                var updateModel = new UpdateVoucherViewModel
                {
                    Voucherid = voucher.Voucherid,
                    Code = voucher.Code,
                    DiscountValue = voucher.DiscountValue ?? 0,
                    DiscountType = voucher.DiscountType ?? 0,
                    StartDate = voucher.StartDate ?? DateTime.Now,
                    EndDate = voucher.EndDate ?? DateTime.Now.AddDays(30),
                    UsageLimit = voucher.UsageLimit ?? 100,
                    UsedCount = voucher.UsedCount ?? 0,
                    MinOrder = voucher.MinOrder ?? 0,
                    IsRoot = voucher.IsRoot ?? true,
                    MaxDiscount = voucher.MaxDiscount,
                    Point = voucher.Point,
                    IsActive = voucher.IsActive ?? true,
                    Metadata = voucher.Metadata
                };

                return View(updateModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin voucher");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi lấy thông tin voucher. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateVoucherViewModel model)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var response = await _voucherService.UpdateVoucherAsync(model, token);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Cập nhật voucher thành công";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật voucher");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi cập nhật voucher. Vui lòng thử lại sau.";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var token = HttpContext.Session.GetString("Token");
            if (token == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                var response = await _voucherService.DeleteVoucherAsync(id, token);
                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Xóa voucher thành công";
                }
                else
                {
                    TempData["ErrorMessage"] = response.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa voucher");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa voucher. Vui lòng thử lại sau.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
