using System;
using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Voucher
{
    public class VoucherViewModel
    {
        public string Voucherid { get; set; }

        public string Code { get; set; }

        public decimal? DiscountValue { get; set; }

        public int? DiscountType { get; set; }

        public string DiscountTypeText => DiscountType == 0 ? "Phần trăm" : "Giá trị cố định";

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? UsageLimit { get; set; }

        public int? UsedCount { get; set; }

        public decimal? MinOrder { get; set; }

        public bool? IsRoot { get; set; }

        public decimal? MaxDiscount { get; set; }

        public int? Point { get; set; }

        public bool? IsActive { get; set; }

        public string StatusText => IsActive == true ? "Đang hoạt động" : "Không hoạt động";

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Metadata { get; set; }
    }

    public class CreateVoucherViewModel
    {
        [Required(ErrorMessage = "Mã voucher không được để trống")]
        [StringLength(50, ErrorMessage = "Mã voucher không được vượt quá 50 ký tự")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Giá trị giảm giá không được để trống")]
        [Range(0.01, 100000000, ErrorMessage = "Giá trị giảm giá phải lớn hơn 0")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Loại giảm giá không được để trống")]
        public int DiscountType { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Giới hạn sử dụng không được để trống")]
        [Range(1, 1000000, ErrorMessage = "Giới hạn sử dụng phải lớn hơn 0")]
        public int UsageLimit { get; set; }

        [Required(ErrorMessage = "Giá trị đơn hàng tối thiểu không được để trống")]
        [Range(0, 100000000, ErrorMessage = "Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0")]
        public decimal MinOrder { get; set; }

        [Required(ErrorMessage = "Loại voucher không được để trống")]
        public bool IsRoot { get; set; } = true;

        [Range(0, 100000000, ErrorMessage = "Giảm giá tối đa phải lớn hơn hoặc bằng 0")]
        public decimal? MaxDiscount { get; set; }

        [Range(0, 1000000, ErrorMessage = "Điểm phải lớn hơn hoặc bằng 0")]
        public int? Point { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public bool IsActive { get; set; } = true;

        public string Metadata { get; set; }
    }

    public class UpdateVoucherViewModel
    {
        public string Voucherid { get; set; }

        [Required(ErrorMessage = "Mã voucher không được để trống")]
        [StringLength(50, ErrorMessage = "Mã voucher không được vượt quá 50 ký tự")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Giá trị giảm giá không được để trống")]
        [Range(0.01, 100000000, ErrorMessage = "Giá trị giảm giá phải lớn hơn 0")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Loại giảm giá không được để trống")]
        public int DiscountType { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Giới hạn sử dụng không được để trống")]
        [Range(1, 1000000, ErrorMessage = "Giới hạn sử dụng phải lớn hơn 0")]
        public int UsageLimit { get; set; }

        public int UsedCount { get; set; }

        [Required(ErrorMessage = "Giá trị đơn hàng tối thiểu không được để trống")]
        [Range(0, 100000000, ErrorMessage = "Giá trị đơn hàng tối thiểu phải lớn hơn hoặc bằng 0")]
        public decimal MinOrder { get; set; }

        [Required(ErrorMessage = "Loại voucher không được để trống")]
        public bool IsRoot { get; set; }

        [Range(0, 100000000, ErrorMessage = "Giảm giá tối đa phải lớn hơn hoặc bằng 0")]
        public decimal? MaxDiscount { get; set; }

        [Range(0, 1000000, ErrorMessage = "Điểm phải lớn hơn hoặc bằng 0")]
        public int? Point { get; set; }

        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public bool IsActive { get; set; }

        public string Metadata { get; set; }
    }

    public class VoucherQueryViewModel
    {
        /// <summary>
        /// ID của khách hàng (dùng cho API lấy voucher của khách hàng)
        /// </summary>
        public string CustomerId { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public string SortBy { get; set; } = "CreatedAt";
        public bool SortAscending { get; set; } = false;
        public bool? IsActive { get; set; }
        public int? DiscountType { get; set; }
        public bool? IsRoot { get; set; }
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
    }
}
