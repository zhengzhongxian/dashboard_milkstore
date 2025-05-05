using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard_MilkStore.Models.Product
{
    public class UpdateProductDTO
    {
        public string? ProductName { get; set; }
        public string? Brand { get; set; }
        public string? Sku { get; set; }
        public string? Bar { get; set; }
        public int? Stockquantity { get; set; }
        public string? Unit { get; set; }
        public string? Description { get; set; }
        public string? StatusId { get; set; }
        public bool? IsActive { get; set; }
    }
}
