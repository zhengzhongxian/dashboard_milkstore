﻿using Dashboard_MilkStore.Models.Admin;
using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Customer;

namespace Dashboard_MilkStore.Services.Admin
{
    public interface IAdminService
    {
        /// <summary>
        /// Lấy danh sách người dùng có vai trò là Admin và Staff có phân trang
        /// </summary>
        Task<ServiceResponse<PaginatedResult<AdminViewModel>>> GetAdminStaffUsersAsync(AdminQueryViewModel query, string token);

        /// <summary>
        /// Lấy thông tin chi tiết của một người dùng có vai trò là Admin hoặc Staff
        /// </summary>
        Task<ServiceResponse<AdminViewModel>> GetAdminStaffDetailAsync(string userId, string token);

        /// <summary>
        /// Cập nhật thông tin của một người dùng có vai trò là Admin hoặc Staff
        /// </summary>
        Task<ServiceResponse<string>> UpdateAdminStaffAsync(string userId, UpdateAdminViewModel model, string token);

        /// <summary>
        /// Cập nhật toàn bộ thông tin của một khách hàng (bao gồm cả coupon và mật khẩu)
        /// </summary>
        Task<ServiceResponse<string>> UpdateCustomerFullAsync(string customerId, UpdateCustomerAdminViewModel model, string token);

        /// <summary>
        /// Lấy danh sách các vai trò
        /// </summary>
        Task<ServiceResponse<List<Models.Admin.RoleViewModel>>> GetRolesAsync(string token);

        /// <summary>
        /// Lấy danh sách các trạng thái người dùng
        /// </summary>
        Task<ServiceResponse<List<Models.Admin.UserStatusViewModel>>> GetUserStatusesAsync(string token);
    }
}
