﻿using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Admin
{
    public class AdminViewModel
    {
        public string UserId { get; set; }
        
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }
        
        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; }
        
        [Display(Name = "Đã xóa")]
        public bool IsDeleted { get; set; }
        
        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; }
        
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }
        
        [Display(Name = "Mã trạng thái")]
        public string StatusId { get; set; }
        
        [Display(Name = "Trạng thái")]
        public string StatusName { get; set; }
        
        [Display(Name = "Mã vai trò")]
        public string RoleId { get; set; }
        
        [Display(Name = "Vai trò")]
        public string RoleName { get; set; }
    }

    public class AdminQueryViewModel
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortAscending { get; set; } = false;
        public string? RoleFilter { get; set; }
    }

    public class UpdateAdminViewModel
    {
        public string UserId { get; set; }
        
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Display(Name = "Mật khẩu mới")]
        public string? Password { get; set; }
        
        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Mã trạng thái")]
        public string? StatusId { get; set; }
        
        [Display(Name = "Mã vai trò")]
        public string? RoleId { get; set; }
        
        // Danh sách các vai trò và trạng thái để hiển thị trong dropdown
        public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
        public List<UserStatusViewModel> UserStatuses { get; set; } = new List<UserStatusViewModel>();
    }
    
    public class UpdateCustomerAdminViewModel
    {
        public string CustomerId { get; set; }
        
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Display(Name = "Mật khẩu mới")]
        public string? Password { get; set; }
        
        [Display(Name = "Họ")]
        public string? Surname { get; set; }
        
        [Display(Name = "Tên đệm")]
        public string? Middlename { get; set; }
        
        [Display(Name = "Tên")]
        public string? Firstname { get; set; }
        
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; }
        
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }
        
        [Display(Name = "Địa chỉ")]
        public string? Address { get; set; }
        
        [Display(Name = "Ảnh đại diện")]
        public string? Avatar { get; set; }
        
        [Display(Name = "Ảnh đại diện mới")]
        public string? AvatarBase64 { get; set; }
        
        [Display(Name = "Ngày sinh")]
        public DateTime? Dob { get; set; }
        
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }
        
        [Display(Name = "Điểm tích lũy")]
        public int? Coupoun { get; set; }
        
        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; } = true;
        
        [Display(Name = "Mã trạng thái")]
        public string? StatusId { get; set; }
        
        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; }
        
        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }
        
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
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
