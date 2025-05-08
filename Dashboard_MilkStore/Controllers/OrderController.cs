using Dashboard_MilkStore.Models.Order;
using Dashboard_MilkStore.Services.Order;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IOrderStatusService _orderStatusService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            IOrderStatusService orderStatusService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _orderStatusService = orderStatusService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 10,
            string sortBy = "OrderDate",
            bool sortAscending = false,
            string statusId = null,
            string searchTerm = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            try
            {
                // Check if user is logged in
                if (HttpContext.Session.GetString("Token") == null)
                {
                    var token = HttpContext.Session.GetString("Token");
                    Console.WriteLine($"token được gửi đi trong order là: {token}");
                    return RedirectToAction("Login", "Account");
                }

                // Lấy danh sách trạng thái đơn hàng
                var statusResponse = await _orderStatusService.GetAllOrderStatusesAsync();
                ViewBag.OrderStatuses = statusResponse.Success ? statusResponse.Data : new System.Collections.Generic.List<OrderStatusViewModel>();

                var query = new OrderQueryViewModel
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    StatusId = statusId,
                    SearchTerm = searchTerm,
                    StartDate = startDate,
                    EndDate = endDate,
                    SortBy = sortBy,
                    SortAscending = sortAscending,
                };

                var result = await _orderService.GetPaginatedOrdersAsync(query);

                // Pass the query parameters to the view for maintaining state
                ViewBag.CurrentPageNumber = pageNumber;
                ViewBag.CurrentPageSize = pageSize;
                ViewBag.CurrentSortBy = sortBy;
                ViewBag.CurrentSortAscending = sortAscending;
                ViewBag.CurrentStatusId = statusId;
                ViewBag.CurrentSearchTerm = searchTerm;
                ViewBag.CurrentStartDate = startDate?.ToString("yyyy-MM-dd");
                ViewBag.CurrentEndDate = endDate?.ToString("yyyy-MM-dd");

                if (!result.Success)
                {
                    TempData["ErrorMessage"] = result.Message ?? "Không thể lấy dữ liệu đơn hàng";
                }
                else if (result.Data?.Items == null || result.Data.Items.Count == 0)
                {
                    TempData["InfoMessage"] = "Không có đơn hàng nào phù hợp với tiêu chí tìm kiếm";
                }

                return View(result.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Order Index action");
                TempData["ErrorMessage"] = $"Error loading orders: {ex.Message}";
                return View(new Models.Common.PaginatedResult<OrderViewModel>());
            }
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                // Check if user is logged in
                if (HttpContext.Session.GetString("Token") == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Order ID is required");
                }

                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound("Order not found");
                }

                // Lấy danh sách trạng thái đơn hàng
                var statusResponse = await _orderStatusService.GetAllOrderStatusesAsync();
                if (statusResponse.Success && statusResponse.Data != null)
                {
                    var status = statusResponse.Data.Find(s => s.StatusId == order.StatusId);
                    if (status != null)
                    {
                        order.StatusName = status.Name;
                    }

                    // Truyền danh sách trạng thái đơn hàng vào ViewBag để hiển thị trong dropdown
                    ViewBag.OrderStatuses = statusResponse.Data;
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Order Details action");
                TempData["ErrorMessage"] = $"Error loading order details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string orderId, string statusId)
        {
            try
            {
                // Check if user is logged in
                if (HttpContext.Session.GetString("Token") == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (string.IsNullOrEmpty(orderId))
                {
                    return BadRequest("Order ID is required");
                }

                if (string.IsNullOrEmpty(statusId))
                {
                    return BadRequest("Status ID is required");
                }

                var result = await _orderService.UpdateOrderStatusAsync(orderId, statusId);

                if (result.Success)
                {
                    TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công";
                    return RedirectToAction(nameof(Details), new { id = orderId });
                }
                else
                {
                    TempData["ErrorMessage"] = result.Message ?? "Không thể cập nhật trạng thái đơn hàng";
                    return RedirectToAction(nameof(Details), new { id = orderId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStatus action");
                TempData["ErrorMessage"] = $"Lỗi khi cập nhật trạng thái đơn hàng: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = orderId });
            }
        }
    }
}
