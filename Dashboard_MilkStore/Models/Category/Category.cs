using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Category
{
    public class Category
    {
        public string Categoryid { get; set; } = string.Empty;

        [Display(Name = "Tên danh mục")]
        public string? CategoryName { get; set; }

        [Display(Name = "Độ ưu tiên")]
        public int? Priority { get; set; }

        [Display(Name = "Danh mục cha")]
        public string? Parentid { get; set; }

        [Display(Name = "Tên danh mục cha")]
        public string? ParentName { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Metadata")]
        public string? Metadata { get; set; }
    }
}
