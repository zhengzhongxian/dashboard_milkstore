using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Product
{
    public class CreateImageDTO
    {
        [Required(ErrorMessage = "ProductId là bắt buộc")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "ImageData là bắt buộc")]
        public string ImageData { get; set; }

        public string? Order { get; set; }
    }

    public class UpdateImageDTO
    {
        [Required(ErrorMessage = "ImageData là bắt buộc")]
        public string ImageData { get; set; } // Base64 encoded image
    }

    public class UpdateImageOrderDTO
    {
        [Required(ErrorMessage = "Order là bắt buộc")]
        public string Order { get; set; }
    }
}
