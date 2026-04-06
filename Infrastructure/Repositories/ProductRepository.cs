using System;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<string>> GetBrandsAsync()
    {
        var brands = await _context.Products
            .Select(brand => brand.ProductBrand)
            .Distinct()
            .ToListAsync();

        return brands;
    }

    public async Task<IReadOnlyCollection<string>> GetTypesAsync()
    {
        var types = await _context.Products
            .Select(type => type.ProductType)
            .Distinct()
            .ToListAsync();

        return types;
    }

    public async Task<IReadOnlyCollection<Product>> GetProductsByBrandAsync(string brandName)
    {
        var normalizedBrandName = brandName.ToLower();

        var products = await _context.Products
            .Where(p => p.ProductBrand.ToLower() == normalizedBrandName)
            .ToListAsync();

        return products;
    }

    public async Task<IReadOnlyCollection<Product>> GetProductsByTypeAsync(string typeName)
    {
        var normalizedTypeName = typeName.ToLower();

        var products = await _context.Products
            .Where(p => p.ProductType.ToLower() == normalizedTypeName)
            .ToListAsync();

        return products;
    }

    public async Task<IReadOnlyCollection<Product>> GetProductsByASpecificBrandAndTypeAsync(string brand, string type)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(brand))
        {
            var normalizedBrand = brand.ToLower();
            query = query.Where(product => product.ProductBrand.ToLower() == normalizedBrand);
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            var normalizedType = type.ToLower();
            query = query.Where(product => product.ProductType.ToLower() == normalizedType);
        }

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyCollection<Product>> GetProductsSortedByPriceAsync(string sort)
    {
        var query = _context.Products.AsQueryable();

        query = sort switch
        {
            "priceAsc" => query.OrderBy(product => product.ProductPrice),
            "priceDesc" => query.OrderByDescending(product => product.ProductPrice),
            _ => query.OrderBy(product => product.ProductName)
        };

        return await query.ToListAsync();
    }
}
