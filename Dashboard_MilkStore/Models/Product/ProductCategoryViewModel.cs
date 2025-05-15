﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dashboard_MilkStore.Models.Product
{
    public class ProductCategoryViewModel
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class CreateProductCategoryViewModel
    {
        [Required(ErrorMessage = "ProductId không được để trống")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "CategoryId không được để trống")]
        public string CategoryId { get; set; }
    }

    public class ProductCategoryChangesViewModel
    {
        public List<string> DeletedCategoryIds { get; set; } = new List<string>();
        public List<string> NewCategoryIds { get; set; } = new List<string>();
    }
}
