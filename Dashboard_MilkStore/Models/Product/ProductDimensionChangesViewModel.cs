using System.Collections.Generic;

namespace Dashboard_MilkStore.Models.Product
{
    public class ProductDimensionChangesViewModel
    {
        public List<DimensionViewModel> UpdatedDimensions { get; set; } = new List<DimensionViewModel>();
        public List<string> DeletedDimensionIds { get; set; } = new List<string>();
        public List<DimensionViewModel> NewDimensions { get; set; } = new List<DimensionViewModel>();
    }

    public class DimensionViewModel
    {
        public string DimensionId { get; set; }
        public decimal? LengthValue { get; set; }
        public decimal? WidthValue { get; set; }
        public decimal? HeightValue { get; set; }
        public decimal? WeightValue { get; set; }
        public string Metadata { get; set; }
    }
}
