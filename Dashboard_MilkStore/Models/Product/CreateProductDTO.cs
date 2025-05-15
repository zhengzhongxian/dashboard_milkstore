using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;
using Dashboard_MilkStore.Models.Product;

namespace Dashboard_MilkStore.Models.Product
{
    public class CreateProductDTO
    {
        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá 200 ký tự")]
        public string ProductName { get; set; } = string.Empty;

        [Display(Name = "Thương hiệu")]
        public string? Brand { get; set; }

        [Display(Name = "Mã SKU")]
        public string? Sku { get; set; }

        [Display(Name = "Mã vạch")]
        public string? Bar { get; set; }

        [Display(Name = "Số lượng tồn kho")]
        [Required(ErrorMessage = "Số lượng trong kho là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng trong kho phải là số dương")]
        public int StockQuantity { get; set; }

        [Display(Name = "Đơn vị")]
        public string? Unit { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [Display(Name = "Trạng thái")]
        public string? StatusId { get; set; }

        [Display(Name = "Kích hoạt")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Metadata")]
        public string? Metadata { get; set; }

        public List<string> CategoryIds { get; set; } = new List<string>();
        public List<CreateProductPriceDTO> ProductPrices { get; set; } = new List<CreateProductPriceDTO>();
        public List<CreateImageDTONew> Images { get; set; } = new List<CreateImageDTONew>();
        public List<CreateDimensionDTONew> Dimensions { get; set; } = new List<CreateDimensionDTONew>();
    }
}
