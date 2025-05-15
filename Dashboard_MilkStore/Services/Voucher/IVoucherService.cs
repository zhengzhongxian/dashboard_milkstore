using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Voucher;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Services.Voucher
{
    public interface IVoucherService
    {
        /// <summary>
        /// Lấy danh sách voucher có phân trang và lọc
        /// </summary>
        Task<ServiceResponse<PaginatedResult<VoucherViewModel>>> GetVouchersAsync(VoucherQueryViewModel query, string token);

        /// <summary>
        /// Lấy thông tin chi tiết của voucher
        /// </summary>
        Task<ServiceResponse<VoucherViewModel>> GetVoucherDetailAsync(string voucherId, string token);

        /// <summary>
        /// Tạo voucher mới
        /// </summary>
        Task<ServiceResponse<VoucherViewModel>> CreateVoucherAsync(CreateVoucherViewModel model, string token);

        /// <summary>
        /// Cập nhật thông tin voucher
        /// </summary>
        Task<ServiceResponse<VoucherViewModel>> UpdateVoucherAsync(UpdateVoucherViewModel model, string token);

        /// <summary>
        /// Xóa voucher
        /// </summary>
        Task<ServiceResponse<bool>> DeleteVoucherAsync(string voucherId, string token);

        /// <summary>
        /// Lấy danh sách voucher của khách hàng từ metadata
        /// </summary>
        /// <param name="query">Tham số truy vấn</param>
        /// <param name="token">Token xác thực</param>
        /// <returns>Danh sách voucher của khách hàng</returns>
        Task<ServiceResponse<PaginatedResult<CustomerVoucherViewModel>>> GetCustomerVouchersAsync(VoucherQueryViewModel query, string token);
    }
}
