using System;

namespace Dashboard_MilkStore.Models.Order
{
    public class OrderStatusViewModel
    {
        public string StatusId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
