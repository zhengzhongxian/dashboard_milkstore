using System;
using System.Collections.Generic;

namespace Dashboard_MilkStore.Models.Order
{
    public class OrderViewModel
    {
        public string OrderId { get; set; }
        public string OrderNumber { get; set; }
        public string CustomerId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string ShippingAddress { get; set; }
        public decimal? ShippingFee { get; set; }
        public string ShippingCode { get; set; }
        public decimal? TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string StatusId { get; set; }
        public string StatusName { get; set; } // Thêm trường này để hiển thị tên trạng thái
        public bool? IsSuccess { get; set; }
        public List<OrderDetailViewModel> OrderDetails { get; set; } = new List<OrderDetailViewModel>();
    }

    public class OrderDetailViewModel
    {
        public string OrderDetailId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal? ProductPrice { get; set; }
        public int? Quantity { get; set; }
        public decimal? SubTotal { get; set; }
    }
}
