﻿using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Customer
{
    public class CustomerDetailViewModel
    {
        // Thông tin từ bảng User
        public string UserId { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; }

        [Display(Name = "Đã xóa")]
        public bool? IsDeleted { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Mã trạng thái")]
        public string StatusId { get; set; }

        [Display(Name = "Trạng thái")]
        public string StatusName { get; set; }

        // Thông tin từ bảng Customer
        public string CustomerId { get; set; }

        [Display(Name = "Họ")]
        public string Surname { get; set; }

        [Display(Name = "Tên đệm")]
        public string Middlename { get; set; }

        [Display(Name = "Tên")]
        public string Firstname { get; set; }

        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Ảnh đại diện")]
        public string Avatar { get; set; }

        [Display(Name = "Mã ảnh")]
        public string AvatarPublicId { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? Dob { get; set; }

        [Display(Name = "Giới tính")]
        public string Gender { get; set; }

        [Display(Name = "Điểm tích lũy")]
        public int? Coupoun { get; set; }

        // Thông tin cho form cập nhật
        [Display(Name = "Ảnh đại diện mới")]
        public string AvatarBase64 { get; set; }

        [Display(Name = "Vai trò")]
        public string RoleId { get; set; }

        [Display(Name = "Tên vai trò")]
        public string RoleName { get; set; }

        // Danh sách các vai trò và trạng thái để hiển thị trong dropdown
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
        public List<UserStatusViewModel> UserStatuses { get; set; } = new List<UserStatusViewModel>();
    }

    public class RoleViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }

    public class UserStatusViewModel
    {
        public string StatusId { get; set; }
        public string Name { get; set; } // Đổi tên từ Name thành StatusName để phù hợp với view
        public string Description { get; set; }
    }
}
