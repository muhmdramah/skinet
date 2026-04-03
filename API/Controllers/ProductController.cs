using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Reader;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _unitOfWork.Products.GetAllAsync();

            if (!products.Any())
                return NotFound("There's no products right now... Try again later!");

            return Ok(products);
        }

        [HttpGet("{productId:int}")]
        public async Task<IActionResult> GetProduct(int productId)
        {
            var products = await _unitOfWork.Products.GetByIdAsync(productId);

            if (products is null)
                return NotFound($"product with id {productId} was not found!");

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product product) 
        {
            if(product == null)
                return BadRequest("Please enter a valid Product!");

            await _unitOfWork.Products.AddAsync(product);
            _unitOfWork.Complete();

            return Ok($"Product with id: {product.Id} was created successfully!");
        }

        [HttpDelete("{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var currentProduct = await _unitOfWork.Products.GetByIdAsync(productId);

            if (currentProduct is null)
                return NotFound($"Product with id: {productId} is already not fount in the context!");

            _unitOfWork.Products.Delete(currentProduct);
            _unitOfWork.Complete();

            return Ok($"Product with id: {productId} deleted successfully!");
        }

        [HttpPut("{productId:int}")]
        public async Task<IActionResult> DeleteProduct(int productId, [FromBody] Product product)
        {
            var currentProduct = await _unitOfWork.Products.GetByIdAsync(productId);

            if (currentProduct is null)
                return NotFound($"Product with id: {productId} was not fount in the context!");

            currentProduct.ProductName = product.ProductName;
            currentProduct.ProductDescription = product.ProductDescription;
            currentProduct.ProductPrice = product.ProductPrice;
            currentProduct.ProductPictureUrl = product.ProductPictureUrl;
            currentProduct.ProductType = product.ProductType;
            currentProduct.ProductBrand = product.ProductBrand;
            currentProduct.ProductQuantityInStock = product.ProductQuantityInStock;

            _unitOfWork.Products.Update(currentProduct);
            _unitOfWork.Complete();
            return Ok($"Product with id: {productId} updated successfully!");
        }
    }
}
