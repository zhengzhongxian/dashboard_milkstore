namespace Dashboard_MilkStore.Models.Product
{
    public class ProductPriceChangesViewModel
    {
        public List<string> DeletedPriceIds { get; set; } = new List<string>();
        public List<NewPriceViewModel> NewPrices { get; set; } = new List<NewPriceViewModel>();
        public List<UpdatedPriceViewModel> UpdatedPrices { get; set; } = new List<UpdatedPriceViewModel>();
    }

    public class NewPriceViewModel
    {
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdatedPriceViewModel
    {
        public string PriceId { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsActive { get; set; }
    }
}
