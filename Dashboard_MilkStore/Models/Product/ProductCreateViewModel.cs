using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Product
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Thương hiệu")]
        public string? Brand1 { get; set; }

        [Display(Name = "Mã SKU")]
        public string? Sku { get; set; }

        [Display(Name = "Mã vạch")]
        public string? Bar { get; set; }

        [Display(Name = "Số lượng tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int? StockQuantity { get; set; }

        [Display(Name = "Đơn vị")]
        public string? Unit1 { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Trạng thái")]
        public string? StatusId { get; set; }

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        // Danh sách hình ảnh mới
        public List<ImageDTO> Images { get; set; } = new List<ImageDTO>();

        // Thông tin giá
        [Display(Name = "Giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal? Price { get; set; }

        [Display(Name = "Đặt làm giá mặc định")]
        public bool IsDefaultPrice { get; set; } = true;

        [Display(Name = "Kích hoạt giá")]
        public bool IsActivePrice { get; set; } = true;

        // Thông tin kích thước
        [Display(Name = "Chiều dài (cm)")]
        public decimal? LengthValue { get; set; }

        [Display(Name = "Chiều rộng (cm)")]
        public decimal? WidthValue { get; set; }

        [Display(Name = "Chiều cao (cm)")]
        public decimal? HeightValue { get; set; }

        [Display(Name = "Cân nặng (kg)")]
        public decimal? WeightValue { get; set; }

        // Danh sách giá và kích thước (cho trường hợp nhiều giá/kích thước)
        public List<ProductPriceDTOCreate> Prices { get; set; } = new List<ProductPriceDTOCreate>();
        public List<Dimension> Dimensions { get; set; } = new List<Dimension>();

        // Danh sách dropdown
        public List<SelectListItem> Statuses { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Units { get; set; } = new List<SelectListItem>();
    }
}
