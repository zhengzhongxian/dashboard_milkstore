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
    }
}
