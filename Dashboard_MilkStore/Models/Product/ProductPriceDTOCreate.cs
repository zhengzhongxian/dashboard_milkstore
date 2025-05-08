namespace Dashboard_MilkStore.Models.Product
{
    public class ProductPriceDTOCreate
    {
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }
}
