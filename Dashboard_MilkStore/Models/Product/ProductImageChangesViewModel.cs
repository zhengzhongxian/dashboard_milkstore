namespace Dashboard_MilkStore.Models.Product
{
    public class ProductImageChangesViewModel
    {
        public List<string> DeletedImageIds { get; set; } = new List<string>();
        public List<NewImageViewModel> NewImages { get; set; } = new List<NewImageViewModel>();
        public List<UpdatedOrderViewModel> UpdatedOrders { get; set; } = new List<UpdatedOrderViewModel>();
    }

    public class NewImageViewModel
    {
        public string ImageData { get; set; } = string.Empty; // Base64 encoded image
        public string Order { get; set; } = "0";
    }

    public class UpdatedOrderViewModel
    {
        public string ImageId { get; set; } = string.Empty;
        public string Order { get; set; } = "0";
    }

    public class BatchImageOrderUpdate
    {
        public List<UpdatedOrderViewModel> Updates { get; set; } = new List<UpdatedOrderViewModel>();
    }
}
