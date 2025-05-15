using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Order
{
    public interface IOrderService
    {
        Task<List<OrderViewModel>> GetAllOrdersAsync();
        Task<OrderViewModel> GetOrderByIdAsync(string id);
        Task<ServiceResponse<PaginatedResult<OrderViewModel>>> GetPaginatedOrdersAsync(OrderQueryViewModel query);
        Task<ServiceResponse<OrderViewModel>> UpdateOrderStatusAsync(string orderId, string statusId);

        /// <summary>
        /// Cập nhật trạng thái đơn hàng bất kỳ (chỉ dành cho admin)
        /// </summary>
        /// <param name="orderId">ID của đơn hàng</param>
        /// <param name="statusId">ID trạng thái mới</param>
        /// <returns>Kết quả cập nhật trạng thái</returns>
        Task<ServiceResponse<OrderViewModel>> AdminUpdateOrderStatusAsync(string orderId, string statusId);

        /// <summary>
        /// Lấy lịch sử đơn hàng của một khách hàng (dành cho admin)
        /// </summary>
        /// <param name="customerId">ID của khách hàng</param>
        /// <param name="pageNumber">Số trang</param>
        /// <param name="pageSize">Kích thước trang</param>
        /// <returns>Danh sách đơn hàng có phân trang</returns>
        Task<ServiceResponse<PaginatedResult<OrderViewModel>>> GetCustomerOrderHistoryAsync(string customerId, int pageNumber = 1, int pageSize = 10);
    }
}
