using Dashboard_MilkStore.Models.Product;
using Dashboard_MilkStore.Services.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Controllers
{
    public class ProductStatisticsController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<ProductStatisticsController> _logger;

        public ProductStatisticsController(
            IAuthService authService,
            ILogger<ProductStatisticsController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public IActionResult Index(string id, int year = 0)
        {
            try
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                var token = HttpContext.Session.GetString("Token");
                if (token == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Kiểm tra vai trò của người dùng
                string role = _authService.GetRoleFromToken(token);
                bool isAdmin = role == "d3f3c3b3-5b5b-4b4b-9b9b-7b7b7b7b7b7b"; // ID của Admin

                // Chỉ cho phép Admin truy cập
                if (!isAdmin)
                {
                    TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này";
                    return RedirectToAction("Index", "Home");
                }

                // Nếu không có ID sản phẩm, chuyển hướng về trang danh sách sản phẩm
                if (string.IsNullOrEmpty(id))
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm";
                    return RedirectToAction("Index", "Product");
                }

                // Nếu không có năm, sử dụng năm hiện tại
                if (year <= 0)
                {
                    year = DateTime.Now.Year;
                }

                // Tạo model cho view
                var model = new ProductStatisticsViewModel
                {
                    ProductId = id,
                    Year = year
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi hiển thị trang thống kê sản phẩm");
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
                return RedirectToAction("Index", "Product");
            }
        }
    }
}
