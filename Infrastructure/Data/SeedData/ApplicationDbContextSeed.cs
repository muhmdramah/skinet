using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Core.Entities.SeedData
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (!context.Products.Any())
            { 
                var productsJson = await File
                        .ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");

                // convert the json file into a list of products 
                var productsList = JsonSerializer.Deserialize<List<Product>>(productsJson);

                if (productsList is null) return;

                context.AddRange(productsList);
                await context.SaveChangesAsync();
            }
        }
    }
}
