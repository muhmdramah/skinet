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
}
