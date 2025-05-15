using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Category
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên danh mục phải từ 2 đến 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string CategoryName { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Độ ưu tiên phải là số không âm")]
        [Display(Name = "Độ ưu tiên")]
        public int? Priority { get; set; }

        [Display(Name = "Danh mục cha")]
        public string? Parentid { get; set; }

        [Display(Name = "Metadata")]
        public string? Metadata { get; set; }

        // Danh sách các danh mục cha để hiển thị trong dropdown
        public IEnumerable<SelectListItem>? Parents { get; set; }
    }
}
