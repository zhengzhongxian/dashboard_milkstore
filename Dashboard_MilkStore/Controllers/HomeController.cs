using System.Diagnostics;
using Dashboard_MilkStore.Models;
using Dashboard_MilkStore.Models.Home;
using Dashboard_MilkStore.Models.Statistics;
using Dashboard_MilkStore.Services.Statistics;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard_MilkStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStatisticsService _statisticsService;

        public HomeController(ILogger<HomeController> logger, IStatisticsService statisticsService)
        {
            _logger = logger;
            _statisticsService = statisticsService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                if (HttpContext.Session.GetString("Token") == null)
                {
                    // Chuyển hướng đến trang đăng nhập nếu không có token
                    return RedirectToAction("Login", "Account");
                }

                // Lấy năm hiện tại
                int currentYear = DateTime.Now.Year;

                // Gọi API để lấy doanh thu theo tháng - không cần truyền token nữa
                var revenueResponse = await _statisticsService.GetMonthlyRevenueForYearAsync(currentYear);

                // Gọi API để lấy thống kê dashboard
                var dashboardStats = await _statisticsService.GetDashboardStatsAsync();

                // Tạo view model
                var viewModel = new HomeViewModel
                {
                    YearlyRevenue = revenueResponse.Data,
                    CurrentYear = currentYear,
                    DashboardStats = dashboardStats
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu doanh thu");
                return View(new HomeViewModel { CurrentYear = DateTime.Now.Year });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductSalesForMonth(int month, int year)
        {
            try
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                if (HttpContext.Session.GetString("Token") == null)
                {
                    return Unauthorized(new { Success = false, Message = "Unauthorized" });
                }

                // Gọi API để lấy doanh số sản phẩm theo tháng
                var response = await _statisticsService.GetProductSalesForMonthAsync(month, year);

                if (!response.Success)
                {
                    return BadRequest(response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu doanh số sản phẩm");
                return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                if (HttpContext.Session.GetString("Token") == null)
                {
                    return Unauthorized(new { Success = false, Message = "Unauthorized" });
                }

                // Gọi API để lấy thống kê dashboard
                var dashboardStats = await _statisticsService.GetDashboardStatsAsync();

                return Ok(new {
                    Success = true,
                    Data = dashboardStats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy dữ liệu thống kê dashboard");
                return StatusCode(500, new { Success = false, Message = $"Lỗi: {ex.Message}" });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
