using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTOs.Requests
{
    public record UpdateProductPriceRequest
    {
        public decimal ProductPrice { get; set; }
    }
}
