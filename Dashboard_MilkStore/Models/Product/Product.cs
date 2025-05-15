namespace Dashboard_MilkStore.Models.Product
{
    public class Product
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public int? SoldQuantity { get; set; }
        public int? Rate { get; set; }
        public string Bar { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Brand1 { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string UnitId { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string StatusId { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public decimal? PriceActive { get; set; }
        public decimal? PriceDefault { get; set; }
        public List<ImageDTO> ImageDTOs { get; set; } = new();
        public List<Dimension> Dimensions { get; set; } = new();
        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public bool? IsActive { get; set; }

        public bool? IsDeleted { get; set; }
    }

    public class ImageDTO
    {
        public string ImageId { get; set; } = string.Empty;
        public string ImageData { get; set; } = string.Empty;
        public string Order { get; set; }
    }

    public class Dimension
    {
        public string DimensionId { get; set; } = string.Empty;
        public decimal? WeightValue { get; set; }
        public decimal? HeightValue { get; set; }
        public decimal? WidthValue { get; set; }
        public decimal? LengthValue { get; set; }
    }

    public class ProductMetadata
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
        public int FirstItemOnPage { get; set; }
    }

    public class ProductResponse
    {
        public ProductMetadata Metadata { get; set; } = new();
        public List<Product> Items { get; set; } = new();
    }
}