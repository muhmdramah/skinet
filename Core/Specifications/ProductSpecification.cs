using Core.Entities;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Specifications
{
    public class ProductSpecification : BaseSpecification<Product>
    {
        public ProductSpecification(string? brand, string? type) : base(criteria: x => 
            (string.IsNullOrWhiteSpace(brand) || x.ProductBrand == brand) &&
            (string.IsNullOrWhiteSpace(type) || x.ProductType == type)
        )
        {
            
        }
    }
}
