using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Product
{
    public class CreateImageDTONew
    {
        [Required(ErrorMessage = "ImageData là bắt buộc")]
        public string ImageData { get; set; }

        public string? Order { get; set; }
    }
}
