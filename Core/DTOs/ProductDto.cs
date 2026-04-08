using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs
{
    public record ProductDto
    {
        public required string ProductName { get; set; }
        public required string ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public required string ProductPictureUrl { get; set; }
        public required string ProductType { get; set; }
        public required string ProductBrand { get; set; }
        public int ProductQuantityInStock { get; set; }
    }
}
