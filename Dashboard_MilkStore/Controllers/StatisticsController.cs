using Dashboard_MilkStore.Services.Statistics;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MonthlyRevenue(int? year)
        {
            // Nếu không có năm được chỉ định, sử dụng năm hiện tại
            int requestedYear = year ?? DateTime.Now.Year;

            // Gọi API để lấy doanh thu theo tháng - không cần truyền token nữa
            var response = await _statisticsService.GetMonthlyRevenueForYearAsync(requestedYear);

            // Truyền dữ liệu vào view
            ViewBag.Year = requestedYear;
            ViewBag.Success = response.Success;
            ViewBag.Message = response.Message;

            return View(response.Data);
        }
    }
}
