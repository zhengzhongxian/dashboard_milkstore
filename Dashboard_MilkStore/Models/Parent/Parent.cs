using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Parent
{
    public class Parent
    {
        public string Parentid { get; set; } = string.Empty;

        [Display(Name = "Tên danh mục cha")]
        public string? ParentName { get; set; }

        [Display(Name = "Độ ưu tiên")]
        public int? Priority { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? UpdatedAt { get; set; }
    }
}
