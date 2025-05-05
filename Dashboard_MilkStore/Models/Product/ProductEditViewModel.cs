using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Product
{
    public class ProductEditViewModel
    {
        public string ProductId { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm")]
        [Display(Name = "Tên sản phẩm")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn kho")]
        [Display(Name = "Số lượng tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int? StockQuantity { get; set; }

        [Display(Name = "Mã vạch")]
        public string? Bar { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã SKU")]
        [Display(Name = "Mã SKU")]
        public string? Sku { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thương hiệu")]
        [Display(Name = "Thương hiệu")]
        public string? Brand1 { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đơn vị")]
        [Display(Name = "Đơn vị")]
        public string? Unit1 { get; set; }

        [Display(Name = "Trạng thái hoạt động")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string? StatusId { get; set; }

        // Image properties
        [Display(Name = "Hình ảnh sản phẩm")]
        public List<ImageDTO> Images { get; set; } = new List<ImageDTO>();
        
        public List<IFormFile>? NewImages { get; set; }

        // Navigation properties for dropdowns
        public IEnumerable<SelectListItem>? Brands { get; set; }
        public IEnumerable<SelectListItem>? Units { get; set; }
        public IEnumerable<SelectListItem>? Statuses { get; set; }

        // Metadata
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Thêm thông tin giá
        public List<ProductPriceDTO> Prices { get; set; } = new List<ProductPriceDTO>();
        public List<Dimension> Dimensions { get; set; } = new List<Dimension>();
        public decimal? DefaultPrice { get; set; }
        public decimal? ActivePrice { get; set; }
    }

    public class ProductPriceDTO
    {
        public string Ppsid { get; set; }
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductStatusDTO
    {
        public string? StatusId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }
    }
} 