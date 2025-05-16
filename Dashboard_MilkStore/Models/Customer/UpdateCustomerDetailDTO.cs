using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Customer
{
    public class UpdateCustomerDetailDTO
    {
        // Thông tin từ bảng User
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool? IsActive { get; set; }

        [Display(Name = "Mã trạng thái")]
        public string StatusId { get; set; }

        [Display(Name = "Vai trò")]
        public string RoleId { get; set; }

        // Thông tin từ bảng Customer
        [Display(Name = "Họ")]
        public string Surname { get; set; }

        [Display(Name = "Tên đệm")]
        public string Middlename { get; set; }

        [Display(Name = "Tên")]
        public string Firstname { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        // Ảnh đại diện dưới dạng base64
        [Display(Name = "Ảnh đại diện mới")]
        public string AvatarBase64 { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? Dob { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Điểm tích lũy")]
        public int? Coupoun { get; set; }
    }
}
