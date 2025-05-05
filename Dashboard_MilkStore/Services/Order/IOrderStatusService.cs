using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Order;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Order
{
    public interface IOrderStatusService
    {
        Task<ServiceResponse<List<OrderStatusViewModel>>> GetAllOrderStatusesAsync();
    }
}
