using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Customer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Customer
{
    public interface ICustomerService
    {
        Task<ServiceResponse<PaginatedResult<UserViewModel>>> GetUsersAsync(UserQueryViewModel query, string token);

        /// <summary>
        /// Lấy thông tin chi tiết của khách hàng
        /// </summary>
        Task<ServiceResponse<CustomerDetailViewModel>> GetCustomerDetailAsync(string customerId, string token);

        /// <summary>
        /// Cập nhật thông tin chi tiết của khách hàng
        /// </summary>
        Task<ServiceResponse<bool>> UpdateCustomerDetailAsync(string customerId, CustomerDetailViewModel model, string token);

        /// <summary>
        /// Lấy danh sách các vai trò
        /// </summary>
        Task<ServiceResponse<List<RoleViewModel>>> GetRolesAsync(string token);

        /// <summary>
        /// Lấy danh sách các trạng thái người dùng
        /// </summary>
        Task<ServiceResponse<List<UserStatusViewModel>>> GetUserStatusesAsync(string token);
    }
}
